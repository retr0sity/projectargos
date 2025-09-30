using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// NPC dialogue that can be triggered either by player interaction OR by entering a trigger zone.
/// FIXED: Better protection against rapid re-triggering
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SimpleNPCDialogue : MonoBehaviour
{
    [Header("Activation Mode")]
    [SerializeField] private bool useTriggerMode = false;
    
    [Header("Dialogue")]
    [SerializeField] private string speakerName = "NPC";
    [TextArea(3, 5)]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private bool oneTimeOnly = true;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueComplete;
    
    private bool hasBeenTalkedTo = false;
    private bool isCurrentlyInDialogue = false; // FIX: Prevent re-entry during dialogue
    
    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            if (useTriggerMode)
            {
                col.isTrigger = true;
                if (gameObject.tag == "Interactable")
                    gameObject.tag = "Untagged";
            }
            else
            {
                col.isTrigger = false;
                if (gameObject.tag == "Untagged")
                    gameObject.tag = "Interactable";
            }
        }
        else
        {
            Debug.LogError($"SimpleNPCDialogue on {gameObject.name} is missing a Collider2D component!");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTriggerMode) return;
        if (!other.CompareTag("Player")) return;
        
        StartDialogue();
    }
    
    public void OnInteract()
    {
        if (useTriggerMode) return;
        
        StartDialogue();
    }
    
    void StartDialogue()
    {
        // FIX: Don't allow re-triggering while already in dialogue
        if (isCurrentlyInDialogue)
        {
            Debug.Log($"{gameObject.name}: Already in dialogue, ignoring trigger");
            return;
        }
        
        if (oneTimeOnly && hasBeenTalkedTo)
        {
            Debug.Log($"{gameObject.name}: Already talked to, ignoring");
            return;
        }
        
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
        
        // FIX: Set both flags to prevent re-entry
        hasBeenTalkedTo = true;
        isCurrentlyInDialogue = true;
        
        Debug.Log($"{gameObject.name}: Starting dialogue");
        
        DialogueManager.Instance.StartDialogue(dialogueLines, speakerName, OnDialogueFinished);
    }
    
    void OnDialogueFinished()
    {
        Debug.Log($"{gameObject.name}: Dialogue finished");
        
        isCurrentlyInDialogue = false; // FIX: Reset dialogue state
        
        onDialogueComplete?.Invoke();
        
        if (oneTimeOnly)
        {
            if (useTriggerMode)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.tag = "Untagged";
            }
        }
    }
    
    public void ResetDialogue()
    {
        hasBeenTalkedTo = false;
        isCurrentlyInDialogue = false; // FIX: Also reset dialogue state
        
        if (useTriggerMode)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.tag = "Interactable";
        }
    }
    
    public void SetTriggerMode(bool enableTrigger)
    {
        useTriggerMode = enableTrigger;
        
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