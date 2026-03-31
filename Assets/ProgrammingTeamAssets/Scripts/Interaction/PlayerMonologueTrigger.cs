using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PlayerMonologueTrigger : MonoBehaviour
{
    [Header("Monologue Lines")]
    [TextArea(2, 4)]
    [SerializeField] private string[] monologueLines;
    [SerializeField] private bool oneTimeOnly = true;

    [Header("Persistent ID")]
    [Tooltip("Unique ID for this trigger. Auto-generated if left empty.")]
    [SerializeField] private string triggerID = "";

    [Header("Events")]
    [SerializeField] private UnityEvent onMonologueShown;
    [Header("Bad Ending Timer")]
    [SerializeField] private bool startsBadEndingTimer = false;
    [SerializeField] private float badEndingMinutes = 5f;

    private bool isCurrentlyShowing = false;

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void Start()
    {
        if (string.IsNullOrEmpty(triggerID))
            triggerID = $"{gameObject.scene.name}_{gameObject.name}";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            ShowMonologue();
    }

    void ShowMonologue()
    {
        if (isCurrentlyShowing) return;

        if (oneTimeOnly && GameStateManager.Instance != null && GameStateManager.Instance.HasTriggerFired(triggerID)) return;

        if (DialogueManager.Instance == null) return;
        if (monologueLines.Length == 0) return;

        if (oneTimeOnly && GameStateManager.Instance != null)
            GameStateManager.Instance.MarkTriggerFired(triggerID);

        isCurrentlyShowing = true;
        DialogueManager.Instance.StartMonologue(monologueLines);
        if (startsBadEndingTimer && GameStateManager.Instance != null)
            GameStateManager.Instance.StartBadEndingTimer(badEndingMinutes * 60f);
        onMonologueShown?.Invoke();
        Invoke("ResetShowingState", 0.5f);
    }

    void ResetShowingState()
    {
        isCurrentlyShowing = false;
    }

    public void ForceShowMonologue()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.ResetTrigger(triggerID);
        ShowMonologue();
    }

    public void ResetTrigger()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.ResetTrigger(triggerID);
        isCurrentlyShowing = false;
    }
}