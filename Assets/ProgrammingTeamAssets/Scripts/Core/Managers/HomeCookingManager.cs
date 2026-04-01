using System.Collections;
using System.Collections.Generic;
using System.Text;
using Core.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Runtime-only Home scene coordinator for fridge storage and cooking flow.
/// Automatically boots when the Home scene loads, builds the house panels
/// under the existing canvas, and wires the blocked-out scene objects.
/// </summary>
public class HomeCookingManager : MonoBehaviour
{
    private const string HomeSceneName = "Home";
    private const string RecipeLibraryResourcePath = "HomeCooking/RecipeLibrary";
    private const string FridgeObjectName = "Fridge";
    private const string KitchenObjectName = "Kitchen";
    private const string ChairObjectName = "Chair";
    private const float MaxCookTimeSeconds = 10f;
    private const float SimpleThresholdSeconds = 3.34f;
    private const float TastyThresholdSeconds = 6.67f;

    private static HomeCookingManager instance;

    public static HomeCookingManager Instance => instance;

    private DialogueManager dialogueManager;
    private RecipeLibrary recipeLibrary;
    private Transform uiRootTransform;
    private GameObject fridgeObject;
    private GameObject kitchenObject;
    private Transform chairTransform;

    private GameObject fridgePanel;
    private TextMeshProUGUI fridgeContentsText;
    private TextMeshProUGUI fridgeStatusText;
    private RectTransform recipeListContent;
    private Button storeAllButton;
    private Button closeFridgeButton;

    private GameObject cookingPanel;
    private TextMeshProUGUI cookingMealText;
    private TextMeshProUGUI cookingTimerText;
    private Button stopCookingButton;

    private Coroutine cookingCoroutine;
    private float cookingStartedAt;
    private bool isCooking;
    private CompletedMealData completedMeal;

    private struct CompletedMealData
    {
        public string recipeName;
        public string resultMealName;
        public MealQualityTier qualityTier;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterSceneBootstrap()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != HomeSceneName)
            return;

        if (FindObjectOfType<HomeCookingManager>() != null)
            return;

        GameObject managerObject = new GameObject(nameof(HomeCookingManager));
        managerObject.AddComponent<HomeCookingManager>();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != HomeSceneName)
        {
            Destroy(gameObject);
            return;
        }

        EnsureSupportManagersForDirectHomeTesting();

        dialogueManager = DialogueManager.Instance;
        recipeLibrary = Resources.Load<RecipeLibrary>(RecipeLibraryResourcePath);
        uiRootTransform = dialogueManager != null ? dialogueManager.GetUIRootTransform() : null;
        fridgeObject = GameObject.Find(FridgeObjectName);
        kitchenObject = GameObject.Find(KitchenObjectName);

        GameObject chairObject = GameObject.Find(ChairObjectName);
        chairTransform = chairObject != null ? chairObject.transform : null;

        if (dialogueManager == null)
        {
            Debug.LogError("HomeCookingManager requires DialogueManager in the Home scene.");
            enabled = false;
            return;
        }

        if (uiRootTransform == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            uiRootTransform = canvas != null ? canvas.transform : null;
        }

        if (uiRootTransform == null)
        {
            Debug.LogError("HomeCookingManager could not find a UI root Canvas in Home.");
            enabled = false;
            return;
        }

        if (recipeLibrary == null)
            Debug.LogError("RecipeLibrary asset not found in Resources/HomeCooking/RecipeLibrary.");

        EnsureInteractableSetup(fridgeObject, true);
        EnsureInteractableSetup(kitchenObject, false);
        BuildPanelsIfNeeded();
        RefreshFridgePanel();
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void OpenFridge()
    {
        if (!enabled || isCooking)
            return;

        if (fridgePanel != null && fridgePanel.activeSelf)
            return;

        BuildPanelsIfNeeded();
        RefreshFridgePanel();
        dialogueManager.BeginExternalUI();
        fridgePanel.SetActive(true);
        SelectButton(storeAllButton, closeFridgeButton);
    }

    public void UseKitchen()
    {
        if (!enabled)
            return;

        if (isCooking)
            return;

        FridgeManager.PendingMealData pendingMeal = FridgeManager.Instance.PendingMeal;
        if (pendingMeal == null)
        {
            if (!dialogueManager.IsDialogueActive())
                dialogueManager.StartDialogue(
                    new[] { "Choose a recipe from the fridge first." },
                    string.Empty);
            return;
        }

        BuildPanelsIfNeeded();
        dialogueManager.BeginExternalUI();
        cookingPanel.SetActive(true);
        cookingMealText.text = pendingMeal.resultMealName;
        UpdateCookingTimerText(0f);
        stopCookingButton.interactable = true;
        SelectButton(stopCookingButton);

        isCooking = true;
        cookingStartedAt = Time.time;

        if (cookingCoroutine != null)
            StopCoroutine(cookingCoroutine);

        cookingCoroutine = StartCoroutine(CookingRoutine(pendingMeal));
    }

    public void CloseFridge()
    {
        if (fridgePanel == null || !fridgePanel.activeSelf)
            return;

        fridgePanel.SetActive(false);
        dialogueManager.EndExternalUI();
    }

    private void EnsureSupportManagersForDirectHomeTesting()
    {
        if (InventoryManager.Instance == null)
            new GameObject("InventoryManager").AddComponent<InventoryManager>();

        if (MoodManager.Instance == null)
            new GameObject("MoodManager").AddComponent<MoodManager>();

        _ = FridgeManager.Instance;
    }

    private void EnsureInteractableSetup(GameObject targetObject, bool isFridge)
    {
        if (targetObject == null)
        {
            Debug.LogError($"HomeCookingManager could not find scene object '{(isFridge ? FridgeObjectName : KitchenObjectName)}'.");
            return;
        }

        if (targetObject.tag != "Interactable")
            targetObject.tag = "Interactable";

        BoxCollider2D collider = targetObject.GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = targetObject.AddComponent<BoxCollider2D>();

        collider.isTrigger = true;

        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
            collider.size = spriteRenderer.sprite.bounds.size;

        if (isFridge)
        {
            if (targetObject.GetComponent<FridgeInteractable>() == null)
                targetObject.AddComponent<FridgeInteractable>();
        }
        else
        {
            if (targetObject.GetComponent<KitchenInteractable>() == null)
                targetObject.AddComponent<KitchenInteractable>();
        }
    }

    private void BuildPanelsIfNeeded()
    {
        if (fridgePanel == null)
            BuildFridgePanel();

        if (cookingPanel == null)
            BuildCookingPanel();
    }

    private void BuildFridgePanel()
    {
        fridgePanel = CreatePanel("FridgePanel", new Vector2(1120f, 700f));
        fridgePanel.SetActive(false);

        CreateText(fridgePanel.transform, "FridgeTitle", "Fridge", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(420f, 60f), 34f, TextAlignmentOptions.Center);
        fridgeContentsText = CreateText(fridgePanel.transform, "FridgeContents", string.Empty, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -100f), new Vector2(360f, 420f), 24f, TextAlignmentOptions.TopLeft);
        fridgeStatusText = CreateText(fridgePanel.transform, "FridgeStatus", string.Empty, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(24f, 96f), new Vector2(360f, 150f), 22f, TextAlignmentOptions.TopLeft);

        storeAllButton = CreateButton(fridgePanel.transform, "StoreAllButton", "Store All Ingredients", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(220f, 36f), new Vector2(320f, 60f));
        storeAllButton.onClick.AddListener(StoreAllIngredients);

        closeFridgeButton = CreateButton(fridgePanel.transform, "CloseFridgeButton", "Close", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-120f, 36f), new Vector2(180f, 60f));
        closeFridgeButton.onClick.AddListener(CloseFridge);

        recipeListContent = CreateRecipeList(fridgePanel.transform);

        // Keep the footer buttons above the scroll view so they remain clickable.
        storeAllButton.transform.SetAsLastSibling();
        closeFridgeButton.transform.SetAsLastSibling();
    }

    private void BuildCookingPanel()
    {
        cookingPanel = CreatePanel("CookingPanel", new Vector2(620f, 300f));
        cookingPanel.SetActive(false);

        CreateText(cookingPanel.transform, "CookingTitle", "Cooking", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(320f, 50f), 30f, TextAlignmentOptions.Center);
        cookingMealText = CreateText(cookingPanel.transform, "CookingMealText", string.Empty, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(520f, 60f), 28f, TextAlignmentOptions.Center);
        cookingTimerText = CreateText(cookingPanel.transform, "CookingTimerText", "0.0 / 10.0s", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), new Vector2(260f, 50f), 24f, TextAlignmentOptions.Center);

        stopCookingButton = CreateButton(cookingPanel.transform, "StopCookingButton", "Stop Cooking", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(240f, 60f));
        stopCookingButton.onClick.AddListener(StopCookingEarly);
    }

    private RectTransform CreateRecipeList(Transform parent)
    {
        GameObject scrollObject = new GameObject("RecipeScroll", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        scrollObject.transform.SetParent(parent, false);

        RectTransform scrollRectTransform = scrollObject.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(1f, 0f);
        scrollRectTransform.anchorMax = new Vector2(1f, 1f);
        scrollRectTransform.pivot = new Vector2(1f, 0.5f);
        scrollRectTransform.anchoredPosition = new Vector2(-24f, -70f);
        scrollRectTransform.sizeDelta = new Vector2(640f, -150f);

        Image scrollImage = scrollObject.GetComponent<Image>();
        scrollImage.color = new Color(0f, 0f, 0f, 0.18f);

        GameObject viewportObject = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
        viewportObject.transform.SetParent(scrollObject.transform, false);
        RectTransform viewportRect = viewportObject.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        Image viewportImage = viewportObject.GetComponent<Image>();
        viewportImage.color = new Color(1f, 1f, 1f, 0.02f);
        viewportObject.GetComponent<Mask>().showMaskGraphic = false;

        GameObject contentObject = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentObject.transform.SetParent(viewportObject.transform, false);
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 0f);

        VerticalLayoutGroup layoutGroup = contentObject.GetComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(16, 16, 16, 16);
        layoutGroup.spacing = 12f;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        ContentSizeFitter sizeFitter = contentObject.GetComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = scrollObject.GetComponent<ScrollRect>();
        scrollRect.viewport = viewportRect;
        scrollRect.content = contentRect;
        scrollRect.horizontal = false;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 20f;

        return contentRect;
    }

    private GameObject CreatePanel(string name, Vector2 size)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(uiRootTransform, false);

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = size;

        Image image = panel.GetComponent<Image>();
        image.color = new Color(0.08f, 0.08f, 0.08f, 0.94f);

        return panel;
    }

    private TextMeshProUGUI CreateText(
        Transform parent,
        string name,
        string text,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        float fontSize,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = anchorMin == anchorMax ? anchorMin : new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;

        TextMeshProUGUI textComponent = textObject.GetComponent<TextMeshProUGUI>();
        textComponent.font = TMP_Settings.defaultFontAsset;
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        textComponent.alignment = alignment;
        textComponent.enableWordWrapping = true;
        textComponent.text = text;

        return textComponent;
    }

    private Button CreateButton(
        Transform parent,
        string name,
        string label,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = anchorMin == anchorMax ? anchorMin : new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.24f, 0.36f, 0.52f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.3f, 0.45f, 0.62f, 1f);
        colors.pressedColor = new Color(0.18f, 0.28f, 0.42f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        TextMeshProUGUI buttonText = CreateText(buttonObject.transform, "Label", label, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 24f, TextAlignmentOptions.Center);
        buttonText.margin = new Vector4(18f, 12f, 18f, 12f);

        return button;
    }

    private void RefreshFridgePanel()
    {
        if (fridgePanel == null)
            return;

        // Auto-learn default recipes every refresh — safe since LearnRecipe uses a HashSet
        if (recipeLibrary != null && GameStateManager.Instance != null)
        {
            foreach (RecipeDefinition recipe in recipeLibrary.recipes)
            {
                if (recipe.learnedByDefault)
                    GameStateManager.Instance.LearnRecipe(recipe.recipeName);
            }
        }

        fridgeContentsText.text = FridgeManager.Instance.BuildContentsSummary();

        bool hasIngredientsToStore = InventoryManager.Instance != null && InventoryManager.Instance.HasAnyIngredients();
        storeAllButton.interactable = hasIngredientsToStore;

        if (FridgeManager.Instance.HasPendingMeal())
        {
            fridgeStatusText.text = $"Pending meal: {FridgeManager.Instance.PendingMeal.resultMealName}\nFinish cooking it before choosing another recipe.";
        }
        else
        {
            fridgeStatusText.text = hasIngredientsToStore
                ? "Store carried ingredients or choose a recipe to cook."
                : "Choose a recipe from what is already in the fridge.";
        }

        RebuildRecipeButtons();
    }

    private void RebuildRecipeButtons()
{
    if (recipeListContent == null)
        return;

    // Clear existing buttons first
    for (int i = recipeListContent.childCount - 1; i >= 0; i--)
        DestroyImmediate(recipeListContent.GetChild(i).gameObject);

    if (recipeLibrary == null || recipeLibrary.recipes.Count == 0)
    {
        CreateText(recipeListContent, "NoRecipesText", "No recipes configured.",
            Vector2.zero, Vector2.one, Vector2.zero, new Vector2(0f, 100f), 24f, TextAlignmentOptions.Center);
        return;
    }

    bool anyLearned = false;
    foreach (RecipeDefinition recipe in recipeLibrary.recipes)
    {
        if (!GameStateManager.Instance.IsRecipeLearned(recipe.recipeName))
            continue;

        anyLearned = true;
        Button recipeButton = CreateRecipeButton(recipe);
        recipeButton.transform.SetParent(recipeListContent, false);
    }

    if (!anyLearned)
    {
        CreateText(recipeListContent, "NoRecipesText", "No recipes learned yet.\nFind recipes out in the world.",
            Vector2.zero, Vector2.one, Vector2.zero, new Vector2(0f, 100f), 22f, TextAlignmentOptions.Center);
    }
}

    private Button CreateRecipeButton(RecipeDefinition recipe)
    {
        bool hasPendingMeal = FridgeManager.Instance.HasPendingMeal();
        bool canCook = !hasPendingMeal && FridgeManager.Instance.CanCook(recipe);
        List<string> missingIngredients = FridgeManager.Instance.GetMissingIngredients(recipe);

        StringBuilder labelBuilder = new StringBuilder();
        labelBuilder.Append(recipe.resultMealName);
        labelBuilder.AppendLine();
        labelBuilder.Append(canCook ? "Ready" : $"Missing: {FormatMissingIngredients(missingIngredients)}");

        Button button = CreateButton(recipeListContent, $"{recipe.recipeName}Button", labelBuilder.ToString(), new Vector2(0f, 1f), new Vector2(1f, 1f), Vector2.zero, new Vector2(0f, 90f));
        LayoutElement layoutElement = button.gameObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 96f;
        layoutElement.flexibleWidth = 1f;

        if (hasPendingMeal)
        {
            button.interactable = false;
        }
        else if (!canCook)
        {
            button.onClick.AddListener(() =>
            {
                fridgeStatusText.text = $"Missing ingredients for {recipe.resultMealName}: {FormatMissingIngredients(missingIngredients)}";
            });
        }
        else
        {
            button.onClick.AddListener(() => SelectRecipe(recipe));
        }

        return button;
    }

    private void StoreAllIngredients()
    {
        if (InventoryManager.Instance == null)
        {
            fridgeStatusText.text = "No player inventory is available.";
            return;
        }

        int storedCount = FridgeManager.Instance.StoreAllIngredientsFromInventory(InventoryManager.Instance);
        fridgeStatusText.text = storedCount > 0
            ? $"Stored {storedCount} ingredient{(storedCount == 1 ? string.Empty : "s")} in the fridge."
            : "No carried ingredients to store.";

        RefreshFridgePanel();
    }

    private void SelectRecipe(RecipeDefinition recipe)
    {
        if (FridgeManager.Instance.HasPendingMeal())
        {
            fridgeStatusText.text = $"Finish cooking {FridgeManager.Instance.PendingMeal.resultMealName} before choosing another recipe.";
            return;
        }

        if (!FridgeManager.Instance.TryStartRecipe(recipe))
        {
            fridgeStatusText.text = $"Missing ingredients for {recipe.resultMealName}: {FormatMissingIngredients(FridgeManager.Instance.GetMissingIngredients(recipe))}";
            return;
        }

        CloseFridge();
        dialogueManager.StartDialogue(
            new[] { $"You take the ingredients for {recipe.resultMealName} to the kitchen." },
            string.Empty);
    }

    private IEnumerator CookingRoutine(FridgeManager.PendingMealData pendingMeal)
    {
        while (true)
        {
            float elapsed = Mathf.Min(Time.time - cookingStartedAt, MaxCookTimeSeconds);
            UpdateCookingTimerText(elapsed);

            if (elapsed >= MaxCookTimeSeconds)
            {
                FinishCooking(pendingMeal, elapsed);
                yield break;
            }

            yield return null;
        }
    }

    private void StopCookingEarly()
    {
        if (!isCooking)
            return;

        FridgeManager.PendingMealData pendingMeal = FridgeManager.Instance.PendingMeal;
        if (pendingMeal == null)
        {
            isCooking = false;
            cookingCoroutine = null;
            EndCookingPanel();
            return;
        }

        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
            cookingCoroutine = null;
        }

        float elapsed = Mathf.Min(Time.time - cookingStartedAt, MaxCookTimeSeconds);
        FinishCooking(pendingMeal, elapsed);
    }

    private void FinishCooking(FridgeManager.PendingMealData pendingMeal, float elapsedSeconds)
    {
        isCooking = false;
        cookingCoroutine = null;
        stopCookingButton.interactable = false;

        completedMeal = new CompletedMealData
        {
            recipeName = pendingMeal.recipeName,
            resultMealName = pendingMeal.resultMealName,
            qualityTier = GetQualityTier(elapsedSeconds)
        };

        EndCookingPanel();
        ShowCompletedMealChoice();
    }

    private void EndCookingPanel()
    {
        if (cookingPanel != null)
            cookingPanel.SetActive(false);

        dialogueManager.EndExternalUI();
    }

    private void ShowCompletedMealChoice()
    {
        string prompt = $"{completedMeal.resultMealName} came out {completedMeal.qualityTier}. What do you want to do?";
        dialogueManager.ShowChoices(
            new[] { "Eat Now", "Store In Inventory" },
            OnCompletedMealChoice,
            prompt);
    }

    private void OnCompletedMealChoice(int choiceIndex)
    {
        switch (choiceIndex)
        {
            case 0:
                EatCompletedMeal();
                break;
            case 1:
                StoreCompletedMeal();
                break;
        }
    }

    private void EatCompletedMeal()
    {
        TeleportPlayerToChair();

        if (MoodManager.Instance != null)
        {
            float bonus = GetMoodBonus(completedMeal.qualityTier);
            MoodManager.Instance.AdjustMood(bonus, $"{completedMeal.resultMealName} lifted your mood!");
        }

        FridgeManager.Instance.ClearPendingMeal();
        completedMeal = default;
    }

    private void StoreCompletedMeal()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("Cannot store cooked meal because InventoryManager is missing.");
            FridgeManager.Instance.ClearPendingMeal();
            completedMeal = default;
            return;
        }

        InventoryManager.Instance.AddCookedMeal(
            completedMeal.resultMealName,
            completedMeal.recipeName,
            completedMeal.qualityTier);

        // Complete quest only if it's the cake, stored in inventory, and timer is still running
        if (completedMeal.recipeName == "Cake Recipe" &&
            GameStateManager.Instance != null &&
            GameStateManager.Instance.badEndingTimerActive &&
            QuestManager.Instance?.currentStage == 2)
        {
            QuestManager.Instance.CompleteQuest();
        }

        FridgeManager.Instance.ClearPendingMeal();
        completedMeal = default;
    }

    private void TeleportPlayerToChair()
    {
        if (chairTransform == null)
        {
            Debug.LogError("HomeCookingManager could not find the Chair transform.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("HomeCookingManager could not find the Player to teleport.");
            return;
        }

        Vector3 chairPosition = chairTransform.position;
        player.transform.position = new Vector3(chairPosition.x, chairPosition.y, player.transform.position.z);
    }

    private void UpdateCookingTimerText(float elapsedSeconds)
    {
        cookingTimerText.text = $"{elapsedSeconds:F1} / {MaxCookTimeSeconds:F1}s";
    }

    private MealQualityTier GetQualityTier(float elapsedSeconds)
    {
        if (elapsedSeconds < SimpleThresholdSeconds)
            return MealQualityTier.Simple;

        if (elapsedSeconds < TastyThresholdSeconds)
            return MealQualityTier.Tasty;

        return MealQualityTier.Excellent;
    }

    private float GetMoodBonus(MealQualityTier qualityTier)
    {
        switch (qualityTier)
        {
            case MealQualityTier.Simple:
                return 5f;
            case MealQualityTier.Tasty:
                return 10f;
            default:
                return 15f;
        }
    }

    private string FormatMissingIngredients(List<string> missingIngredients)
    {
        if (missingIngredients == null || missingIngredients.Count == 0)
            return "None";

        return string.Join(", ", missingIngredients);
    }

    private void SelectButton(params Button[] candidates)
    {
        if (EventSystem.current == null)
            return;

        foreach (Button candidate in candidates)
        {
            if (candidate != null && candidate.interactable && candidate.gameObject.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(candidate.gameObject);
                return;
            }
        }
    }
}
