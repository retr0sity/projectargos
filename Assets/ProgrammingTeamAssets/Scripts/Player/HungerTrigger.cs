using UnityEngine;

public class HungerTrigger : MonoBehaviour
{
    [Tooltip("Leave at 0 to use HungerManager's default hungerChangeAmount.")]
    public float amount = 0f;
    public bool triggerOnce = true;

    [Tooltip("Unique ID for this trigger. Auto-generated if left empty.")]
    [SerializeField] private string triggerID = "";

    void Awake()
    {
        // intentionally empty — scene name not reliable here
    }

    void Start()
    {
        if (string.IsNullOrEmpty(triggerID))
            triggerID = $"{gameObject.scene.name}_{gameObject.name}";
        
        Debug.Log($"[HungerTrigger] Start — ID: '{triggerID}', already fired: {GameStateManager.Instance?.HasTriggerFired(triggerID)}");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (triggerOnce && GameStateManager.Instance != null && GameStateManager.Instance.HasTriggerFired(triggerID))
            return;

        if (triggerOnce && GameStateManager.Instance != null)
            GameStateManager.Instance.MarkTriggerFired(triggerID);

        if (amount <= 0f)
            HungerManager.Instance.DecreaseHunger();
        else
            HungerManager.Instance.DecreaseHunger(amount);
    }
}