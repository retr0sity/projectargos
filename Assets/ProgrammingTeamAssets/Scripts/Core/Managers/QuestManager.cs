using UnityEngine;

/// <summary>
/// Tracks the single demo quest across all scenes.
/// Stage 0 = not started, Stage 1 = find the recipe, Stage 2 = cook the cake, Stage 3 = complete.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public int currentStage { get; private set; } = 0;

    private static readonly string[] stageTitles = new string[]
    {
        "",
        "A Taste of Home",
        "A Taste of Home",
        "A Taste of Home"
    };

    private static readonly string[] stageDescriptions = new string[]
    {
        "",
        "Find a recipe somewhere in the world.",
        "Cook the cake using the recipe you found.",
        "Quest Complete!"
    };

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string GetTitle() => currentStage > 0 ? stageTitles[currentStage] : "";
    public string GetDescription() => currentStage > 0 ? stageDescriptions[currentStage] : "";
    public bool IsActive() => currentStage > 0 && currentStage < 3;
    public bool IsComplete() => currentStage >= 3;

    public void StartQuest()
    {
        if (currentStage != 0) return;
        SetStage(1);
    }

    public void AdvanceToStage2()
    {
        if (currentStage != 1) return;
        SetStage(2);
    }

    public void CompleteQuest()
    {
        if (currentStage != 2) return;
        SetStage(3);
    }

    private void SetStage(int stage)
    {
        currentStage = stage;
        Debug.Log($"[Quest] Advanced to stage {stage}: {GetDescription()}");
    }

    public void ResetQuest()
    {
        currentStage = 0;
    }
}