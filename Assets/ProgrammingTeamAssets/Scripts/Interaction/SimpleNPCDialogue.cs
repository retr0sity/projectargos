using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple linear NPC dialogue interaction.
/// Player presses interact to start dialogue and advance through lines.
/// Requires "Interactable" tag and DialogueManager in scene.
/// </summary>
public class SimpleNPCDialogue : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private string speakerName = "NPC";
    [TextArea(3, 5)]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private bool oneTimeOnly = true;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueComplete;
    
    private bool hasBeenTalkedTo = false;
    
    /// <summary>
    /// Called by InteractionDetector when player presses interact near this NPC
    /// </summary>
    public void OnInteract()
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
        
        // Remove interactable tag so player can't re-interact
        if (oneTimeOnly)
            gameObject.tag = "Untagged";
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
        gameObject.tag = "Interactable";
    }
}