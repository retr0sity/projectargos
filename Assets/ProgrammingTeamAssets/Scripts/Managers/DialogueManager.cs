using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Managers;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 5)]
    public string text;
    public string speakerName;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public int nextDialogueIndex; // -1 to end dialogue
    public UnityEngine.Events.UnityEvent onChoiceSelected;
}

[System.Serializable]
public class DialogueSequence
{
    public DialogueLine[] lines;
    public DialogueChoice[] choices; // If empty, dialogue just ends
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private GameObject continuePrompt;
    
    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Transform choiceButtonContainer;
    
    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.03f;
    
    private Queue<DialogueLine> currentLines = new Queue<DialogueLine>();
    private DialogueChoice[] currentChoices;
    private bool isTyping;
    private bool canContinue;
    private Coroutine typingCoroutine;
    private Action onDialogueComplete;
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
    }
    
    void Start()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent += OnInteractPressed;
        }
    }
    
    public void StartDialogue(DialogueSequence sequence, Action onComplete = null)
    {
        if (sequence == null || sequence.lines.Length == 0) return;
        
        onDialogueComplete = onComplete;
        currentLines.Clear();
        
        foreach (var line in sequence.lines)
        {
            currentLines.Enqueue(line);
        }
        
        currentChoices = sequence.choices;
        
        dialoguePanel.SetActive(true);
        InputManager.Instance?.SetControlLock(true);
        
        DisplayNextLine();
    }
    
    public void StartDialogue(DialogueLine[] lines, Action onComplete = null)
    {
        StartDialogue(new DialogueSequence { lines = lines, choices = null }, onComplete);
    }
    
    void DisplayNextLine()
    {
        if (currentLines.Count == 0)
        {
            if (currentChoices != null && currentChoices.Length > 0)
            {
                ShowChoices();
            }
            else
            {
                EndDialogue();
            }
            return;
        }
        
        var line = currentLines.Dequeue();
        
        if (speakerNameText != null)
        {
            speakerNameText.text = line.speakerName;
        }
        
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }
    
    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        canContinue = false;
        dialogueText.text = "";
        continuePrompt.SetActive(false);
        
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        
        isTyping = false;
        canContinue = true;
        continuePrompt.SetActive(true);
    }
    
    void ShowChoices()
    {
        choicePanel.SetActive(true);
        continuePrompt.SetActive(false);
        
        // Clear old choices
        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create choice buttons
        for (int i = 0; i < currentChoices.Length; i++)
        {
            var choice = currentChoices[i];
            var buttonGO = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            var button = buttonGO.GetComponent<Button>();
            var buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonText.text = choice.choiceText;
            
            button.onClick.AddListener(() => {
                OnChoiceSelected(choice);
            });
        }
    }
    
    void OnChoiceSelected(DialogueChoice choice)
    {
        choicePanel.SetActive(false);
        choice.onChoiceSelected?.Invoke();
        
        if (choice.nextDialogueIndex >= 0)
        {
            // Load next dialogue sequence based on index
            // This would need to be implemented based on your dialogue storage system
        }
        else
        {
            EndDialogue();
        }
    }
    
    void OnInteractPressed()
    {
        if (!dialoguePanel.activeSelf) return;
        
        if (isTyping)
        {
            // Skip to end of current line
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            dialogueText.text = dialogueText.text; // Show full text
            isTyping = false;
            canContinue = true;
            continuePrompt.SetActive(true);
        }
        else if (canContinue)
        {
            DisplayNextLine();
        }
    }
    
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        InputManager.Instance?.SetControlLock(false);
        onDialogueComplete?.Invoke();
    }
    
    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf;
    }
    
    void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent -= OnInteractPressed;
        }
    }
}