using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Managers;

/// <summary>
/// Central manager for all dialogue, monologue, choice, and image interactions.
/// Handles UI display, text typing effects, and player control locking.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue UI")]
	[SerializeField] private GameObject dialoguePanel;
	[SerializeField] private GameObject speakerNamePanel;
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private TextMeshProUGUI speakerNameText;

    [Header("Monologue UI")]
    [SerializeField] private GameObject monologuePanel;
    [SerializeField] private TextMeshProUGUI monologueText;
	[SerializeField] private float timeWhaitMonologue = 2f;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;

    [Header("Image UI")]
    [SerializeField] private GameObject imagePanel;
    [SerializeField] private Image displayImage;

    [Header("Interaction UI")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.03f;

    // State tracking
    private Queue<string> dialogueQueue = new Queue<string>();
    private Coroutine currentDialogue;
    private Action onDialogueComplete;
    private bool isTyping = false;
    private string currentFullLine = "";
    private bool waitingForInput = false;
    
    private Coroutine currentMonologue;
    private Queue<string> monologueQueue = new Queue<string>();
    private bool isMonologueActive = false;
    
    private Coroutine currentImage;
    private bool imageWaitingForDismiss = false;
    
    // Player reference for emergency stop (safety measure for momentum bugs)
    private RigPlayerController playerController;
    private Rigidbody2D playerRigidbody;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        HideAllPanels();
    }

    void Start()
    {
        // Cache player references
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<RigPlayerController>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
        }
    }

    void OnEnable()
    {
        // Subscribe to interact event for advancing dialogue/monologue
        if (InputManager.Instance != null)
            InputManager.Instance.InteractEvent += OnInteractPressed;
    }

    void OnDisable()
    {
        // Clean up subscriptions
        if (InputManager.Instance != null)
            InputManager.Instance.InteractEvent -= OnInteractPressed;
    }

    /// <summary>
    /// Handles interact button press for advancing dialogue/monologue/image
    /// </summary>
   void OnInteractPressed()
{
    Debug.Log($"Interact pressed! Image:{imageWaitingForDismiss} Mono:{isMonologueActive} Dialog:{currentDialogue != null} Waiting:{waitingForInput}");
    
    if (imageWaitingForDismiss)
    {
        imageWaitingForDismiss = false;
        return;
    }
    
    if (isMonologueActive)
    {
        DisplayNextMonologueLine();
        return;
    }

    if (currentDialogue != null && waitingForInput)
    {
        if (isTyping)
        {
            CompleteTyping();
        }
        else
        {
			Debug.Log("Ready to go to the next line!!!");
            DisplayNextLine();
        }
		Debug.Log("Ready to return from interaction");
        return; // ← ADD THIS! Don't continue to other handlers
    }

    if (currentMonologue != null && !isMonologueActive)
    {
        StopCoroutine(currentMonologue);
        HideMonologue();
        currentMonologue = null;
    }
}

    void HideAllPanels()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (monologuePanel) monologuePanel.SetActive(false);
        if (choicePanel) choicePanel.SetActive(false);
        if (imagePanel) imagePanel.SetActive(false);
        if (interactionPrompt) interactionPrompt.SetActive(false);
    }

    // ============================================
    // DIALOGUE SYSTEM (NPC conversations)
    // Locks player controls, requires interact to advance
    // ============================================

    /// <summary>
    /// Starts a new dialogue sequence with the given lines
    /// </summary>
    public void StartDialogue(string[] lines, string speaker = "", Action onComplete = null)
{	
	isMonologueActive = false;
    if (currentDialogue != null)
        StopCoroutine(currentDialogue);

    onDialogueComplete = onComplete;
    dialogueQueue.Clear();
    
    foreach (string line in lines)
        dialogueQueue.Enqueue(line);

    // Show panels
    if (dialoguePanel) dialoguePanel.SetActive(true);
    
    // NEW: Show/hide name panel based on whether there's a speaker
    if (speakerNamePanel) speakerNamePanel.SetActive(!string.IsNullOrEmpty(speaker));
    if (speakerNameText != null) speakerNameText.text = speaker;

    LockPlayerMovement();
    DisplayNextLine();
}

    void DisplayNextLine()
    {
		Debug.Log($"DisplayNextLine called! Queue count: {dialogueQueue.Count}");
        if (dialogueQueue.Count == 0)
        {
			Debug.Log("Queue empty, calling EndDialogue()");
            EndDialogue();
            return;
        }

        currentFullLine = dialogueQueue.Dequeue();
    	Debug.Log($"Dequeued line: '{currentFullLine}'. Remaining in queue: {dialogueQueue.Count}");
        
        if (currentDialogue != null)
            StopCoroutine(currentDialogue);
            
        currentDialogue = StartCoroutine(TypeLine(currentFullLine, dialogueText));
    }

    IEnumerator TypeLine(string line, TextMeshProUGUI textComponent)
	{
   		isTyping = true;
    	waitingForInput = true; // Make sure this is set IMMEDIATELY
    	textComponent.text = "";
    
    	foreach (char letter in line)
    	{
        	textComponent.text += letter;
        	yield return new WaitForSeconds(textSpeed);
    	}
    
    	isTyping = false;
    	//currentDialogue = null; // Clear the coroutine reference
    	// waitingForInput stays TRUE so interact can advance
	}

    void CompleteTyping()
    {
        if (currentDialogue != null)
            StopCoroutine(currentDialogue);
        
        dialogueText.text = currentFullLine;
        isTyping = false;
        //currentDialogue = null;
    }

    void EndDialogue()
	{
		Debug.Log("EndDialogue called!");
    	if (dialoguePanel) dialoguePanel.SetActive(false);
    	if (speakerNamePanel) speakerNamePanel.SetActive(false);
    
    	waitingForInput = false;
    	UnlockPlayerMovement();
    
    	onDialogueComplete?.Invoke();
    	currentDialogue = null;
		Debug.Log("EndDialogue complete");
	}

    // ============================================
    // MONOLOGUE SYSTEM (Internal thoughts)
    // Does NOT lock controls, player can advance with interact
    // ============================================

    /// <summary>
    /// Shows single internal monologue text (doesn't freeze player)
    /// </summary>
    public void ShowMonologue(string text)
    {
        if (currentMonologue != null)
            StopCoroutine(currentMonologue);

        currentMonologue = StartCoroutine(MonologueSequence(text));
    }

    IEnumerator MonologueSequence(string text)
    {
        GameObject panel = monologuePanel ?? dialoguePanel;
        TextMeshProUGUI textComponent = monologueText ?? dialogueText;

        if (panel) panel.SetActive(true);
        
        if (panel == dialoguePanel && speakerNameText != null)
            speakerNameText.text = "";

        textComponent.text = "";
        foreach (char letter in text)
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void HideMonologue()
    {
        GameObject panel = monologuePanel ?? dialoguePanel;
        if (panel) panel.SetActive(false);
    }
    
    /// <summary>
    /// Start a multi-line monologue sequence (auto-triggered, no control lock)
    /// </summary>
    public void StartMonologue(string[] lines)
    {
		// Reset dialogue state when starting monologue
    	if (currentDialogue != null)
    	{
        	StopCoroutine(currentDialogue);
        	currentDialogue = null;
        	waitingForInput = false;
    	}
        if (currentMonologue != null)
            StopCoroutine(currentMonologue);

        monologueQueue.Clear();
        
        foreach (string line in lines)
            monologueQueue.Enqueue(line);

        isMonologueActive = true;
        DisplayNextMonologueLine();
    }

    void DisplayNextMonologueLine()
    {
        if (monologueQueue.Count == 0)
        {
            EndMonologue();
            return;
        }

        string line = monologueQueue.Dequeue();
        
        if (currentMonologue != null)
            StopCoroutine(currentMonologue);
            
        currentMonologue = StartCoroutine(TypeMonologueLine(line));
    }

   IEnumerator TypeMonologueLine(string line)
{
    GameObject panel = monologuePanel ?? dialoguePanel;
    TextMeshProUGUI textComponent = monologueText ?? dialogueText;

    if (panel) panel.SetActive(true);
    if (panel == dialoguePanel && speakerNameText != null)
        speakerNameText.text = "";

    // Type out text
    textComponent.text = "";
    foreach (char letter in line)
    {
        textComponent.text += letter;
        yield return new WaitForSeconds(textSpeed);
    }
    
    // NEW: Wait 4 seconds, then auto-advance
    yield return new WaitForSeconds(timeWhaitMonologue);
    
    // Auto-advance to next line after timeout
    if (isMonologueActive)
    {
        DisplayNextMonologueLine();
    }
}

    void EndMonologue()
    {
        GameObject panel = monologuePanel ?? dialoguePanel;
        if (panel) panel.SetActive(false);
        
        isMonologueActive = false;
        currentMonologue = null;
    }

    // ============================================
    // CHOICE SYSTEM (Branching dialogue)
    // Locks controls until choice is made
    // ============================================

    /// <summary>
    /// Display choice buttons and wait for player selection
    /// </summary>
    public void ShowChoices(string[] choices, Action<int> onChoiceSelected)
    {
        if (choicePanel) choicePanel.SetActive(true);

        LockPlayerControls();

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                TextMeshProUGUI buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText) buttonText.text = choices[i];

                int index = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => {
                    OnChoiceMade(index, onChoiceSelected);
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnChoiceMade(int choiceIndex, Action<int> callback)
    {
        if (choicePanel) choicePanel.SetActive(false);
        UnlockPlayerControls();
        callback?.Invoke(choiceIndex);
    }

    // ============================================
    // IMAGE DISPLAY SYSTEM
    // Shows sprite for duration OR until interact dismisses it
    // ============================================

    /// <summary>
    /// Display an image for a set duration (used for item inspection)
    /// Player can dismiss early by pressing interact
    /// </summary>
    public void ShowImage(Sprite sprite, float duration = 4f)
    {
        if (currentImage != null)
            StopCoroutine(currentImage);
            
        currentImage = StartCoroutine(ImageSequence(sprite, duration));
    }

    IEnumerator ImageSequence(Sprite sprite, float duration)
{
    if (imagePanel) imagePanel.SetActive(true);
    if (displayImage != null) displayImage.sprite = sprite;

    LockPlayerMovement();
    
    // NEW: Wait one frame before allowing dismissal
    // This prevents the interact press that opened the image from immediately closing it
    yield return null;
    
    imageWaitingForDismiss = true;

    float elapsed = 0f;
    while (elapsed < duration && imageWaitingForDismiss)
    {
        elapsed += Time.deltaTime;
        yield return null;
    }

    if (imagePanel) imagePanel.SetActive(false);
    UnlockPlayerMovement();
    imageWaitingForDismiss = false;
    currentImage = null;
}

    // ============================================
    // CONTROL LOCK HELPERS
    // ============================================

    /// <summary>
    /// Lock player controls and stop all movement
    /// SAFETY: Manually stops player as backup for momentum bugs
    /// </summary>
    void LockPlayerControls()
{
    if (InputManager.Instance != null)
        InputManager.Instance.SetControlLock(true);

    if (playerController != null)
        playerController.enabled = false;
    
    if (playerRigidbody != null)
        playerRigidbody.linearVelocity = new Vector2(0f, playerRigidbody.linearVelocity.y);
}

    /// <summary>
    /// Unlock player controls and re-enable movement
    /// </summary>
   void UnlockPlayerControls()
{
    if (InputManager.Instance != null)
        InputManager.Instance.SetControlLock(false);

    if (playerController != null)
        playerController.enabled = true;
}

void LockPlayerMovement()
{
    // Lock ONLY movement, interact still works
    if (InputManager.Instance != null)
        InputManager.Instance.SetMovementLock(true);

    if (playerController != null)
        playerController.enabled = false;
    
    if (playerRigidbody != null)
        playerRigidbody.linearVelocity = new Vector2(0f, playerRigidbody.linearVelocity.y);
}

void UnlockPlayerMovement()
{
    if (InputManager.Instance != null)
        InputManager.Instance.SetMovementLock(false);

    if (playerController != null)
        playerController.enabled = true;
}

    // ============================================
    // UI HELPERS
    // ============================================

    public void ShowInteractionPrompt(bool show)
    {
        if (interactionPrompt) interactionPrompt.SetActive(show);
    }

    public bool IsDialogueActive()
    {
        return (dialoguePanel != null && dialoguePanel.activeSelf) ||
               (choicePanel != null && choicePanel.activeSelf);
    }

    public bool IsAnyUIActive()
    {
        return IsDialogueActive() ||
               (monologuePanel != null && monologuePanel.activeSelf) ||
               (imagePanel != null && imagePanel.activeSelf);
    }
}