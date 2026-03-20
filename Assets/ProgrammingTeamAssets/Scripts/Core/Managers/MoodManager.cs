using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Tracks player mood across the whole game. DontDestroyOnLoad singleton.
/// Call MoodManager.Instance.IncreaseMood() or DecreaseMood() from anywhere.
/// Requires a MoodFloatingText component in each scene that needs to show notifications.
/// </summary>
public class MoodManager : MonoBehaviour
{
    public static MoodManager Instance { get; private set; }

    [Header("Mood Settings")]
    public float mood = 50f;       // starts at 50 out of 100
    public float minMood = 0f;
    public float maxMood = 100f;
    public float moodChangeAmount = 10f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void IncreaseMood()
    {
        AdjustMood(moodChangeAmount, "Mood went up!");
    }

    public void DecreaseMood()
    {
        AdjustMood(-moodChangeAmount, "Mood went down!");
    }

    public void AdjustMood(float delta, string floatingMessage = null)
    {
        mood = Mathf.Clamp(mood + delta, minMood, maxMood);

        if (delta > 0f)
            Debug.Log($"[Mood] Increased to {mood}");
        else if (delta < 0f)
            Debug.Log($"[Mood] Decreased to {mood}");
        else
            Debug.Log($"[Mood] Unchanged at {mood}");

        if (!string.IsNullOrWhiteSpace(floatingMessage))
        {
            MoodFloatingText.ShowIfPresent(floatingMessage);
        }
        else if (delta > 0f)
        {
            MoodFloatingText.ShowIfPresent("Mood went up!");
        }
        else if (delta < 0f)
        {
            MoodFloatingText.ShowIfPresent("Mood went down!");
        }
    }
}
