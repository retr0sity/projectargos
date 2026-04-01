using UnityEngine;
using System.Collections;
using Core.Managers;

/// <summary>
/// A collectible recipe paper. Player presses E to read it — shows a recipe panel.
/// Second press dismisses the panel and permanently learns the recipe.
/// Requires "Interactable" tag. Uses GameStateManager to persist learned state.
/// </summary>
public class RecipePaperPickup : MonoBehaviour
{
    [Header("Recipe")]
    [Tooltip("Must match the recipeName field on your RecipeDefinition asset exactly.")]
    [SerializeField] private string recipeName = "Birthday Cake";

    [Header("Panel Content")]
    [SerializeField] private string paperTitle = "Handwritten Recipe";
    [TextArea(4, 10)]
    [SerializeField] private string recipeText =
        "Olive Oil  x1\n" +
        "Egg        x1\n" +
        "Yogurt     x1\n" +
        "Sugar      x1\n" +
        "Orange     x1\n" +
        "Flour      x1\n\n" +
        "Mix well and bake with love.";

    [Header("Persistent ID")]
    [SerializeField] private string pickupID = "";

    private bool isPanelOpen = false;
    private GameObject panelRoot;

    void Start()
    {
        if (string.IsNullOrEmpty(pickupID))
            pickupID = $"{gameObject.scene.name}_{gameObject.name}";

        gameObject.tag = "Interactable";

        QuestManager.Instance?.StartQuest1(); // <-- add this

        // Hide if already picked up — checked two ways for safety
        if (GameStateManager.Instance != null && 
            (GameStateManager.Instance.HasTriggerFired(pickupID) || 
            GameStateManager.Instance.IsRecipeLearned(recipeName)))
        {
            gameObject.SetActive(false);
        }
    }

    public void OnInteract()
    {
        if (isPanelOpen)
        {
            // Second press — dismiss panel and learn recipe
            ClosePanel();
            LearnRecipe();
        }
        else
        {
            // First press — show the recipe panel
            OpenPanel();
        }
    }

    private void OpenPanel()
{
    isPanelOpen = true;

    // Lock movement only — keep interact available for second press
    if (InputManager.Instance != null)
        InputManager.Instance.SetMovementLock(true);

    panelRoot = BuildPanel();
}

private void ClosePanel()
{
    isPanelOpen = false;

    if (panelRoot != null)
    {
        Destroy(panelRoot);
        panelRoot = null;
    }

    // Unlock movement
    if (InputManager.Instance != null)
        InputManager.Instance.SetMovementLock(false);
}

    private void LearnRecipe()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.LearnRecipe(recipeName);
            GameStateManager.Instance.MarkTriggerFired(pickupID); // <-- add this
        }

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.StartMonologue(new[] { $"I learned how to make {recipeName.Replace(" Recipe", "")}!" });

        gameObject.SetActive(false);
        QuestManager.Instance?.AdvanceQuest1();
    }

    private GameObject BuildPanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[RecipePaper] No Canvas found in scene!");
            return null;
        }

        GameObject panel = new GameObject("RecipePaperPanel", typeof(RectTransform));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.sizeDelta        = new Vector2(360f, 500f);
        rt.anchoredPosition = Vector2.zero;

        UnityEngine.UI.Image bg = panel.AddComponent<UnityEngine.UI.Image>();
        bg.color = new Color(0.96f, 0.92f, 0.8f, 0.97f);

        // Meal name at the very top
        AddTextAnchored(panel.transform, $"— {paperTitle} —", 16f,
            TMPro.FontStyles.Italic, new Color(0.3f, 0.15f, 0f, 0.7f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(16f, -14f), new Vector2(-16f, -38f));

        // Title below meal name
        AddTextAnchored(panel.transform, "Handwritten Recipe", 22f,
            TMPro.FontStyles.Bold, new Color(0.2f, 0.1f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(16f, -40f), new Vector2(-16f, -78f));

        // Divider
        GameObject div = new GameObject("Divider", typeof(RectTransform));
        div.transform.SetParent(panel.transform, false);
        RectTransform divRT = div.GetComponent<RectTransform>();
        divRT.anchorMin        = new Vector2(0.05f, 1f);
        divRT.anchorMax        = new Vector2(0.95f, 1f);
        divRT.pivot            = new Vector2(0.5f, 1f);
        divRT.anchoredPosition = new Vector2(0f, -82f);
        divRT.sizeDelta        = new Vector2(0f, 1f);
        div.AddComponent<UnityEngine.UI.Image>().color = new Color(0.4f, 0.25f, 0.1f, 0.6f);

        // Recipe body — starts below divider
        AddTextAnchored(panel.transform, recipeText, 17f,
            TMPro.FontStyles.Normal, new Color(0.15f, 0.08f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(24f, -90f), new Vector2(-24f, -460f));

        // Dismiss hint pinned to bottom
        AddTextAnchored(panel.transform, "[E] Put it away", 13f,
            TMPro.FontStyles.Italic, new Color(0.4f, 0.3f, 0.1f, 0.8f),
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(16f, 10f), new Vector2(-16f, 34f));

        return panel;
    }

    private void AddTextAnchored(Transform parent, string text, float size,
        TMPro.FontStyles style, Color color,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;

        TMPro.TextMeshProUGUI tmp = go.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text               = text;
        tmp.fontSize           = size;
        tmp.fontStyle          = style;
        tmp.color              = color;
        tmp.alignment          = TMPro.TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;
    }

    private void AddText(Transform parent, string text, float size,
        TMPro.FontStyles style, Color color,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.offsetMin = new Vector2(20f + offsetMin.x, offsetMin.y);
        rt.offsetMax = new Vector2(-20f + offsetMax.x, offsetMax.y);

        TMPro.TextMeshProUGUI tmp = go.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text               = text;
        tmp.fontSize           = size;
        tmp.fontStyle          = style;
        tmp.color              = color;
        tmp.alignment          = TMPro.TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;
    }
}