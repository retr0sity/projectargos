using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// NPC dialogue that can be triggered either by player interaction OR by entering a trigger zone.
/// Choose between interaction mode (requires "Interactable" tag) or trigger mode (auto-starts).
/// Refactor Needed - this is a mess
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SimpleNPCDialogue : MonoBehaviour
{
    [Header("Activation Mode")]
    [SerializeField] private bool useTriggerMode = false; // False = interact mode, True = trigger mode
    
    [Header("Dialogue")]
    [SerializeField] private string speakerName = "NPC";
    [TextArea(3, 5)]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private bool oneTimeOnly = true;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueComplete;
    
    private bool hasBeenTalkedTo = false;
    
    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            if (useTriggerMode)
            {
                // Trigger mode: Set as trigger, no "Interactable" tag
                col.isTrigger = true;
                if (gameObject.tag == "Interactable")
                    gameObject.tag = "Untagged";
            }
            else
            {
                // Interact mode: Not a trigger, needs "Interactable" tag
                col.isTrigger = false;
                if (gameObject.tag == "Untagged")
                    gameObject.tag = "Interactable";
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Only process triggers if in trigger mode
        if (!useTriggerMode) return;
        if (!other.CompareTag("Player")) return;
        
        StartDialogue();
    }
    
    /// <summary>
    /// Called by InteractionDetector when player presses interact (interact mode only)
    /// </summary>
    public void OnInteract()
    {
        // Only process interactions if in interact mode
        if (useTriggerMode) return;
        
        StartDialogue();
    }
    
    /// <summary>
    /// Starts the dialogue sequence (called by either trigger or interact)
    /// </summary>
    void StartDialogue()
    {
        // Check if already talked to (if one-time only)
        if (oneTimeOnly && hasBeenTalkedTo) return;
        
        // Validate dependencies
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }
        
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogWarning($"No dialogue lines set for {gameObject.name}");
            return;
        }
        
        // Mark as talked to BEFORE starting dialogue (prevents double-trigger)
        hasBeenTalkedTo = true;
        
        // Start dialogue (DialogueManager handles control locking)
        DialogueManager.Instance.StartDialogue(dialogueLines, speakerName, OnDialogueFinished);
    }
    
    /// <summary>
    /// Called when dialogue sequence completes
    /// </summary>
    void OnDialogueFinished()
    {
        onDialogueComplete?.Invoke();
        
        // Handle one-time usage
        if (oneTimeOnly)
        {
            if (useTriggerMode)
            {
                // Trigger mode: Disable the entire GameObject
                gameObject.SetActive(false);
            }
            else
            {
                // Interact mode: Remove interactable tag
                gameObject.tag = "Untagged";
            }
        }
    }
    
    // ============================================
    // PUBLIC METHODS (for testing/external control)
    // ============================================
    
    /// <summary>
    /// Reset the dialogue to allow it to be triggered again
    /// </summary>
    public void ResetDialogue()
    {
        hasBeenTalkedTo = false;
        
        if (useTriggerMode)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.tag = "Interactable";
        }
    }
    
    /// <summary>
    /// Switch between trigger and interact modes (for testing)
    /// </summary>
    public void SetTriggerMode(bool enableTrigger)
    {
        useTriggerMode = enableTrigger;
        
        // Update collider and tag settings
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = useTriggerMode;
            
            if (useTriggerMode)
            {
                gameObject.tag = "Untagged";
            }
            else
            {
                gameObject.tag = "Interactable";
            }
        }
    }
}