using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Managers;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel; // The DialogueBox
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerNameText; // Optional
    
    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons; // Array of buttons (2-4)
    [SerializeField] private TextMeshProUGUI[] choiceTexts; // Text on each button
    
    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.03f;
    
    private Queue<string> sentences;
    private bool isTyping;
    private bool canContinue;
    private string currentSentence;
    private Action onDialogueComplete;
    private PlayerController playerController;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        sentences = new Queue<string>();
        
        // Panels should be DISABLED in hierarchy - we'll enable them when needed
        // Don't try to disable them here
    }
    
    void Start()
    {
        // Find player controller
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerController = player.GetComponent<PlayerController>();
            
        // Subscribe to interact for continuing dialogue
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent += OnInteractPressed;
        }
    }
    
    public void StartDialogue(string[] dialogueLines, string speakerName = "", Action onComplete = null)
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;
        
        onDialogueComplete = onComplete;
        sentences.Clear();
        
        foreach (string line in dialogueLines)
        {
            sentences.Enqueue(line);
        }
        
        // Set speaker name if provided
        if (speakerNameText != null && !string.IsNullOrEmpty(speakerName))
        {
            speakerNameText.text = speakerName;
        }
        else if (speakerNameText != null)
        {
            speakerNameText.text = ""; // Clear if no name
        }
        
        // ENABLE the panel (it should be disabled in hierarchy)
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
        
        // Pause player movement
        if (playerController != null)
            playerController.enabled = false;
        
        DisplayNextSentence();
    }
    
    void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        
        string sentence = sentences.Dequeue();
        currentSentence = sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        canContinue = false;
        dialogueText.text = "";
        
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        
        isTyping = false;
        canContinue = true;
    }
    
    public void ShowChoices(string[] choices, Action<int> onChoiceSelected)
    {
        if (choices == null || choices.Length == 0) return;
        
        // ENABLE the choice panel (it should be disabled in hierarchy)
        if (choicePanel != null)
            choicePanel.SetActive(true);
        
        // Setup each button
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = choices[i];
                
                // Remove old listeners and add new one
                int choiceIndex = i; // Capture for closure
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => {
                    OnChoiceSelected(choiceIndex, onChoiceSelected);
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }
    
    void OnChoiceSelected(int choiceIndex, Action<int> callback)
    {
        // DISABLE the choice panel
        if (choicePanel != null)
            choicePanel.SetActive(false);
            
        callback?.Invoke(choiceIndex);
    }
    
    void OnInteractPressed()
    {
        if (!dialoguePanel.activeSelf) return;
        if (choicePanel.activeSelf) return; // Don't advance if choices are shown
        
        if (isTyping)
        {
            // Skip to end of current sentence
            StopAllCoroutines();
            dialogueText.text = currentSentence;
            isTyping = false;
            canContinue = true;
        }
        else if (canContinue)
        {
            DisplayNextSentence();
        }
    }
    
    void EndDialogue()
    {
        // DISABLE the panels
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (choicePanel != null)
            choicePanel.SetActive(false);
        
        // Resume player movement
        if (playerController != null)
            playerController.enabled = true;
            
        onDialogueComplete?.Invoke();
    }
    
    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }
    
    void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent -= OnInteractPressed;
        }
    }
}