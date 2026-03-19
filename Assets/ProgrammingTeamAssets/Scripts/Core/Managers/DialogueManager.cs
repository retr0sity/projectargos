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
/// FIXED: Proper cooldown system to prevent double-triggering
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
    [SerializeField] private TextMeshProUGUI choicePromptText;

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
    private bool isMonologueTyping = false;
    private string currentMonologueFullLine = "";

    
    // FIX: Use coroutine-based cooldown instead of flag
    private Coroutine startCooldownCoroutine;
    
    private Coroutine currentMonologue;
    private Queue<string> monologueQueue = new Queue<string>();
    private bool isMonologueActive = false;
    private bool monologueWaitingForInput = false;
    
    private Coroutine currentImage;
    private bool imageWaitingForDismiss = false;
    private bool capturedDefaultChoicePrompt = false;
    private string defaultChoicePrompt = "Make Your Choice";
    
    // Player reference
    private BasePlayerController playerController;
    private Rigidbody2D playerRigidbody;

    // Keep track of which InputManager instance we are subscribed to.
    private InputManager subscribedInputManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CacheChoicePromptReferenceIfNeeded();
        HideAllPanels();
    }

    void Start()
    {
        CachePlayerReferencesIfNeeded();
        EnsureInputSubscription();
    }

    void OnEnable()
    {
        EnsureInputSubscription();
    }

    void OnDisable()
    {
        RemoveInputSubscription();
    }

    void OnDestroy()
    {
        RemoveInputSubscription();
    }

    void Update()
    {
        EnsureInputSubscription();
    }

    void EnsureInputSubscription()
    {
        InputManager input = InputManager.Instance;

        if (input == null)
            return;

        if (subscribedInputManager == input)
            return;

        // If InputManager instance changed, clean up old subscription first.
        if (subscribedInputManager != null)
            subscribedInputManager.InteractEvent -= OnInteractPressed;

        input.InteractEvent += OnInteractPressed;
        subscribedInputManager = input;
    }

    void RemoveInputSubscription()
    {
        if (subscribedInputManager == null)
            return;

        subscribedInputManager.InteractEvent -= OnInteractPressed;
        subscribedInputManager = null;
    }

    void OnInteractPressed()
    {
        Debug.Log($"Interact pressed! Cooldown:{startCooldownCoroutine != null} Image:{imageWaitingForDismiss} Mono:{isMonologueActive} Dialog:{currentDialogue != null} Waiting:{waitingForInput}");
        
        // FIX: Block ALL interact events during cooldown period
        if (startCooldownCoroutine != null)
        {
            Debug.Log("Blocked by start cooldown");
            return;
        }
        
        if (imageWaitingForDismiss)
        {
            imageWaitingForDismiss = false;
            return;
        }
        
        if (isMonologueActive && monologueWaitingForInput)
        {
            // If monologue is still typing, finish it first
            if (isMonologueTyping)
            {
                isMonologueTyping = false;
                monologueText.text = currentMonologueFullLine;
                return;
            }

            // Otherwise go to next line
            DisplayNextMonologueLine();
            return;
        }


        // FIX: Check waitingForInput first, then handle dialogue state
        if (waitingForInput)
        {
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
                Debug.Log("Ready to go to the next line!");
                DisplayNextLine();
            }
            Debug.Log("Ready to return from interaction");
            return;
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
        if (speakerNamePanel) speakerNamePanel.SetActive(false);
        if (monologuePanel) monologuePanel.SetActive(false);
        if (choicePanel) choicePanel.SetActive(false);
        if (imagePanel) imagePanel.SetActive(false);
        if (interactionPrompt) interactionPrompt.SetActive(false);
    }

    // ============================================
    // DIALOGUE SYSTEM
    // ============================================

    public void StartDialogue(string[] lines, string speaker = "", Action onComplete = null)
    {
        // FIX: Start cooldown coroutine that lasts multiple frames
        if (startCooldownCoroutine != null)
            StopCoroutine(startCooldownCoroutine);
        startCooldownCoroutine = StartCoroutine(StartDialogueCooldown());
        
        // FIX: Explicitly hide interaction prompt when dialogue starts
        ShowInteractionPrompt(false);
        
        // Stop any active monologue
        isMonologueActive = false;
        monologueWaitingForInput = false;
        if (currentMonologue != null)
        {
            StopCoroutine(currentMonologue);
            currentMonologue = null;
        }
        
        // Hide monologue panel
        if (monologuePanel != null)
            monologuePanel.SetActive(false);
        
        // Stop existing dialogue
        if (currentDialogue != null)
        {
            StopCoroutine(currentDialogue);
            currentDialogue = null;
        }

        onDialogueComplete = onComplete;
        dialogueQueue.Clear();
        
        foreach (string line in lines)
            dialogueQueue.Enqueue(line);

        if (dialoguePanel) dialoguePanel.SetActive(true);
        if (speakerNamePanel) speakerNamePanel.SetActive(!string.IsNullOrEmpty(speaker));
        if (speakerNameText != null) speakerNameText.text = speaker;

        // Lock movement
        LockPlayerMovement();
        
        waitingForInput = false;
        DisplayNextLine();
    }

    // FIX: Cooldown coroutine that blocks input for 2 frames
    IEnumerator StartDialogueCooldown()
    {
        yield return null; // Wait 1 frame
        yield return null; // Wait 2 frames
        startCooldownCoroutine = null;
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
        {
            StopCoroutine(currentDialogue);
            currentDialogue = null;
        }
            
        currentDialogue = StartCoroutine(TypeLine(currentFullLine, dialogueText));
    }

    IEnumerator TypeLine(string line, TextMeshProUGUI textComponent)
    {
        isTyping = true;
        textComponent.text = "";
        
        // Wait one frame before allowing input
        yield return null;
        waitingForInput = true;
        
        foreach (char letter in line)
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        
        isTyping = false;
    }

    void CompleteTyping()
    {
        if (currentDialogue != null)
        {
            StopCoroutine(currentDialogue);
            currentDialogue = null;
        }
        
        dialogueText.text = currentFullLine;
        isTyping = false;
    }

    void EndDialogue()
    {
        try
        {
            Debug.Log("EndDialogue called!");
            
            if (dialoguePanel) dialoguePanel.SetActive(false);
            if (speakerNamePanel) speakerNamePanel.SetActive(false);
            
            waitingForInput = false;
            
            onDialogueComplete?.Invoke();
            currentDialogue = null;
            
            Debug.Log("EndDialogue complete");
        }
        finally
        {
            // FIX: Use coroutine to unlock with delay
            StartCoroutine(UnlockPlayerWithDelay());
        }
    }

    // ============================================
    // MONOLOGUE SYSTEM
    // ============================================

    public void ShowMonologue(string text)
    {
        if (currentMonologue != null)
        {
            StopCoroutine(currentMonologue);
            currentMonologue = null;
        }

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
        
        yield return new WaitForSeconds(timeWhaitMonologue);
        
        HideMonologue();
        currentMonologue = null;
    }

    void HideMonologue()
    {
        GameObject panel = monologuePanel ?? dialoguePanel;
        if (panel) panel.SetActive(false);
    }
    
    public void StartMonologue(string[] lines)
    {
        if (currentDialogue != null)
        {
            StopCoroutine(currentDialogue);
            currentDialogue = null;
            waitingForInput = false;
        }
        
        if (currentMonologue != null)
        {
            StopCoroutine(currentMonologue);
            currentMonologue = null;
        }

        monologueQueue.Clear();
        
        foreach (string line in lines)
            monologueQueue.Enqueue(line);

        isMonologueActive = true;
        monologueWaitingForInput = false;
        isMonologueTyping = false;
        currentMonologueFullLine = "";
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
        {
            StopCoroutine(currentMonologue);
            currentMonologue = null;
        }
            
        currentMonologue = StartCoroutine(TypeMonologueLine(line));
    }

    IEnumerator TypeMonologueLine(string line)
    {
        GameObject panel = monologuePanel ?? dialoguePanel;
        TextMeshProUGUI textComponent = monologueText ?? dialogueText;

        if (panel) panel.SetActive(true);
        if (panel == dialoguePanel && speakerNameText != null)
            speakerNameText.text = "";

        currentMonologueFullLine = line;
        textComponent.text = "";
        
        yield return null;
        monologueWaitingForInput = true;
        isMonologueTyping = true;

        foreach (char letter in line)
        {
            // If player pressed interact while typing, finish instantly
            if (!isMonologueActive || !isMonologueTyping)
            {
                textComponent.text = currentMonologueFullLine;
                isMonologueTyping = false;
                yield break;
            }

            textComponent.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        // Done typing
        isMonologueTyping = false;

        // Now wait for input OR timeout — whichever comes first
        float elapsed = 0f;
        while (elapsed < timeWhaitMonologue && monologueWaitingForInput && isMonologueActive)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (isMonologueActive)
            DisplayNextMonologueLine();
    }

    void EndMonologue()
    {
        GameObject panel = monologuePanel ?? dialoguePanel;
        if (panel) panel.SetActive(false);
        
        isMonologueActive = false;
        monologueWaitingForInput = false;
        currentMonologue = null;
    }

    // ============================================
    // CHOICE SYSTEM
    // ============================================

    public void ShowChoices(string[] choices, Action<int> onChoiceSelected, string promptText = null)
    {
        CacheChoicePromptReferenceIfNeeded();

        if (choicePanel) choicePanel.SetActive(true);
        if (choicePromptText != null)
        {
            choicePromptText.text = string.IsNullOrWhiteSpace(promptText)
                ? defaultChoicePrompt
                : promptText;
        }

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
        if (choicePromptText != null)
            choicePromptText.text = defaultChoicePrompt;

        UnlockPlayerControls();
        callback?.Invoke(choiceIndex);
    }

    // ============================================
    // IMAGE SYSTEM
    // ============================================

    public void ShowImage(Sprite sprite, float duration = 4f)
    {
        if (currentImage != null)
        {
            StopCoroutine(currentImage);
            currentImage = null;
        }
            
        currentImage = StartCoroutine(ImageSequence(sprite, duration));
    }

    IEnumerator ImageSequence(Sprite sprite, float duration)
    {
        if (imagePanel) imagePanel.SetActive(true);
        if (displayImage != null) displayImage.sprite = sprite;

        LockPlayerMovement();
        
        yield return null;
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

    void LockPlayerControls()
    {
        CachePlayerReferencesIfNeeded();

        if (InputManager.Instance != null)
            InputManager.Instance.SetControlLock(true);

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        if (playerController != null)
            playerController.enabled = false;
    }

    void UnlockPlayerControls()
    {
        CachePlayerReferencesIfNeeded();

        if (InputManager.Instance != null)
            InputManager.Instance.SetControlLock(false);

        if (playerController != null)
            playerController.enabled = true;
    }

    void LockPlayerMovement()
    {
        CachePlayerReferencesIfNeeded();

        if (InputManager.Instance != null)
            InputManager.Instance.SetMovementLock(true);

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        if (playerController != null)
            playerController.enabled = false;
    }

    void UnlockPlayerMovement()
    {
        CachePlayerReferencesIfNeeded();

        if (InputManager.Instance != null)
            InputManager.Instance.SetMovementLock(false);

        if (playerController != null)
            playerController.enabled = true;
    }

    /// <summary>
    /// FIX: Wait for player to actually release movement keys before unlocking
    /// </summary>
    IEnumerator UnlockPlayerWithDelay()
    {
        CachePlayerReferencesIfNeeded();

        // Clear velocity
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        // Unlock input manager FIRST
        if (InputManager.Instance != null)
            InputManager.Instance.SetMovementLock(false);

        // Wait until movement keys are actually released (or timeout after 2 seconds)
        float timeWaited = 0f;
        float timeout = 2f;
        
        while (timeWaited < timeout)
        {
            // Check if Move action is reading zero (keys released)
            if (InputManager.Instance != null)
            {
                // Try to read current move value from the Input System
                // If it's zero, keys are released
                var moveAction = InputManager.Instance.GetMoveAction();
                if (moveAction != null)
                {
                    Vector2 currentInput = moveAction.ReadValue<Vector2>();
                    if (currentInput.magnitude < 0.1f) // Keys released
                    {
                        break;
                    }
                }
            }
            
            timeWaited += Time.deltaTime;
            yield return null;
        }

        // Wait one more frame to be safe
        yield return null;

        // Clear velocity right before enabling
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        // Now safe to re-enable controller
        if (playerController != null)
            playerController.enabled = true;
    }

    void CachePlayerReferencesIfNeeded()
    {
        if (playerController != null && playerRigidbody != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (playerController == null)
            playerController = player.GetComponent<BasePlayerController>();

        if (playerRigidbody == null)
            playerRigidbody = player.GetComponent<Rigidbody2D>();
    }

    // ============================================
    // UI HELPERS
    // ============================================

    public void ShowInteractionPrompt(bool show)
    {
        if (interactionPrompt) interactionPrompt.SetActive(show);
    }

    void CacheChoicePromptReferenceIfNeeded()
    {
        if (choicePromptText == null && choicePanel != null)
        {
            TextMeshProUGUI[] promptCandidates = choicePanel.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (TextMeshProUGUI candidate in promptCandidates)
            {
                if (candidate == null)
                    continue;

                if (candidate.transform.parent == choicePanel.transform)
                {
                    choicePromptText = candidate;
                    break;
                }
            }
        }

        if (!capturedDefaultChoicePrompt &&
            choicePromptText != null &&
            !string.IsNullOrWhiteSpace(choicePromptText.text))
        {
            defaultChoicePrompt = choicePromptText.text;
            capturedDefaultChoicePrompt = true;
        }
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
