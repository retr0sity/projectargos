using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Automatically triggers multi-line internal monologue when player enters trigger area.
/// Player advances through lines by pressing interact button.
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
    
    void Awake()
    {
        // Ensure this is set to trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Auto-trigger when player enters
        if (other.CompareTag("Player"))
        {
            ShowMonologue();
        }
    }
    
    void ShowMonologue()
    {
        // Check if already triggered
        if (oneTimeOnly && hasTriggered) return;
        if (DialogueManager.Instance == null) return;
        if (monologueLines.Length == 0) return;
        
        hasTriggered = true;
        
        // Start the monologue sequence (DialogueManager handles line advancement)
        DialogueManager.Instance.StartMonologue(monologueLines);
        
        onMonologueShown?.Invoke();
    }
    
    // ============================================
    // PUBLIC METHODS (for external control)
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
    }
}