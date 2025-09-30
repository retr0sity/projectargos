using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Automatically triggers multi-line internal monologue when player enters trigger area.
/// Player advances through lines by pressing interact button.
/// FIXED: Better state management
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlayerMonologueTrigger : MonoBehaviour
{
    [Header("Monologue Lines")]
    [TextArea(2, 4)]
    [SerializeField] private string[] monologueLines;
    [SerializeField] private bool oneTimeOnly = true;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onMonologueShown;
    
    private bool hasTriggered = false;
    private bool isCurrentlyShowing = false; // FIX: Prevent double-trigger
    
    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowMonologue();
        }
    }
    
    void ShowMonologue()
    {
        // FIX: Don't trigger if already showing
        if (isCurrentlyShowing) return;
        
        // Check if already triggered
        if (oneTimeOnly && hasTriggered) return;
        if (DialogueManager.Instance == null) return;
        if (monologueLines.Length == 0) return;
        
        hasTriggered = true;
        isCurrentlyShowing = true; // FIX: Mark as showing
        
        // Start the monologue sequence
        DialogueManager.Instance.StartMonologue(monologueLines);
        
        onMonologueShown?.Invoke();
        
        // FIX: Reset showing state after a delay (monologue is non-blocking)
        // This allows the monologue to play without locking the trigger forever
        Invoke("ResetShowingState", 0.5f);
    }
    
    void ResetShowingState()
    {
        isCurrentlyShowing = false;
    }
    
    // ============================================
    // PUBLIC METHODS
    // ============================================
    
    /// <summary>
    /// Force show the monologue, bypassing the oneTimeOnly check
    /// </summary>
    public void ForceShowMonologue()
    {
        hasTriggered = false;
        ShowMonologue();
    }
    
    /// <summary>
    /// Reset the trigger so it can be activated again
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        isCurrentlyShowing = false; // FIX: Also reset showing state
    }
}