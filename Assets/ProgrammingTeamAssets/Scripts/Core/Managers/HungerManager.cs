using UnityEngine;

/// <summary>
/// Tracks player hunger across the whole game. DontDestroyOnLoad singleton.
/// Call HungerManager.Instance.DecreaseHunger() from events,
/// and HungerManager.Instance.Eat() when the player consumes a meal.
/// </summary>
public class HungerManager : MonoBehaviour
{
    public static HungerManager Instance { get; private set; }

    [Header("Hunger Settings")]
    public float hunger = 100f;
    public float minHunger = 0f;
    public float maxHunger = 100f;
    public float hungerChangeAmount = 20f;

    [Header("Mood Impact")]
    [Tooltip("Mood penalty applied once when hunger hits zero.")]
    public float moodPenaltyOnStarve = 10f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Call from events — uses hungerChangeAmount by default.</summary>
    public void DecreaseHunger(float amount = -1f)
    {
        float delta = amount < 0f ? -hungerChangeAmount : -amount;
        AdjustHunger(delta);
    }

    /// <summary>Call when the player eats a meal. Also boosts mood.</summary>
    public void Eat(float amount = -1f)
    {
        float delta = amount < 0f ? hungerChangeAmount : amount;
        AdjustHunger(delta);

        MoodManager.Instance?.IncreaseMood();
        MoodFloatingText.ShowIfPresent("Ate a meal!");
    }

    public void AdjustHunger(float delta)
    {
        float previous = hunger;
        hunger = Mathf.Clamp(hunger + delta, minHunger, maxHunger);

        if (delta > 0f)
            Debug.Log($"[Hunger] Increased to {hunger}");
        else if (delta < 0f)
            Debug.Log($"[Hunger] Decreased to {hunger}");

        // One-shot mood penalty when hunger first reaches zero
        if (hunger <= minHunger && previous > minHunger)
        {
            Debug.Log("[Hunger] Starving — applying mood penalty.");
            MoodManager.Instance?.AdjustMood(-moodPenaltyOnStarve, "So hungry...");
        }
    }

    /// <summary>Call from GameStateManager.ResetAllState().</summary>
    public void ResetHunger()
    {
        hunger = maxHunger;
    }
}