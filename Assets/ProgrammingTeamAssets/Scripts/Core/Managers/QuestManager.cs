using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [System.Serializable]
    public class QuestStage
    {
        public string title;
        [TextArea(2, 4)]
        public string description;
    }

    [Header("Quest Settings")]
    [SerializeField] private string questName = "A Taste of Home";
    [SerializeField] public QuestStage[] stages = new QuestStage[]
    {
        new QuestStage { title = "A Taste of Home", description = "Find a recipe somewhere in the world." },
        new QuestStage { title = "A Taste of Home", description = "Cook the cake using the recipe you found." }
    };
    [SerializeField] private string completionText = "Quest Complete!";

    public int currentStage { get; private set; } = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string GetTitle() => currentStage > 0 && currentStage <= stages.Length
        ? stages[currentStage - 1].title
        : questName;

    public string GetDescription()
    {
        if (IsComplete()) return completionText;
        if (currentStage > 0 && currentStage <= stages.Length)
            return stages[currentStage - 1].description;
        return "";
    }

    public bool IsActive() => currentStage > 0 && !IsComplete();
    public bool IsComplete() => currentStage > stages.Length;

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
        SetStage(stages.Length + 1);
    }

    private void SetStage(int stage)
    {
        currentStage = stage;
        Debug.Log($"[Quest] Stage {stage}: {GetDescription()}");
    }

    public void ResetQuest()
    {
        currentStage = 0;
    }
}