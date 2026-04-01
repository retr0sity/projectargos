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

    [System.Serializable]
    public class QuestData
    {
        public string questName;
        public QuestStage[] stages;
        public string completionText = "Quest Complete!";
        [HideInInspector] public int currentStage = 0;

        public bool IsStarted() => currentStage > 0;
        public bool IsComplete() => currentStage > stages.Length;
        public bool IsActive() => IsStarted() && !IsComplete();

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

        public void Advance()
        {
            currentStage++;
            Debug.Log($"[Quest: {questName}] Stage {currentStage}: {GetDescription()}");
        }

        public void Reset() => currentStage = 0;
    }

    [Header("Quests")]
    public QuestData quest1 = new QuestData
    {
        questName = "A Taste of Home",
        completionText = "Quest Complete!",
        stages = new QuestStage[]
        {
            new QuestStage { title = "A Taste of Home", description = "Find a recipe somewhere in the world." },
            new QuestStage { title = "A Taste of Home", description = "Cook the birthday cake using the recipe you found." }
        }
    };

    public QuestData quest2 = new QuestData
    {
        questName = "Hungry?",
        completionText = "Quest Complete!",
        stages = new QuestStage[]
        {
            new QuestStage { title = "Hungry?", description = "Eat a cooked meal." }
        }
    };

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Quest 1 ────────────────────────────────────────────────────────
    public void StartQuest1()
    {
        if (quest1.IsStarted()) return;
        quest1.Advance();
    }

    public void AdvanceQuest1()
    {
        if (!quest1.IsActive()) return;
        quest1.Advance();
    }

    public void CompleteQuest1()
    {
        if (quest1.IsComplete()) return;
        while (!quest1.IsComplete()) quest1.Advance();
    }

    public bool IsQuest1Complete() => quest1.IsComplete();

    // ── Quest 2 ────────────────────────────────────────────────────────
    public void StartQuest2()
    {
        if (quest2.IsStarted()) return;
        quest2.Advance();
        Debug.Log("[Quest2] Started — eat a cooked meal.");
    }

    public void CompleteQuest2()
    {
        if (quest2.IsComplete()) return;
        while (!quest2.IsComplete()) quest2.Advance();
    }

    public bool IsQuest2Complete() => quest2.IsComplete();

    // ── Reset ──────────────────────────────────────────────────────────
    public void ResetQuest()
    {
        quest1.Reset();
        quest2.Reset();
    }
}