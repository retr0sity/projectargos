using UnityEngine;

public class HungerTrigger : MonoBehaviour
{
    [Tooltip("Leave at 0 to use HungerManager's default hungerChangeAmount.")]
    public float amount = 0f;
    public bool triggerOnce = true;

    private bool _triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        if (triggerOnce) _triggered = true;

        if (amount <= 0f)
            HungerManager.Instance.DecreaseHunger();
        else
            HungerManager.Instance.DecreaseHunger(amount);
    }
}