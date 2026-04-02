using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    private GameObject _canvasGO;
    private GameObject _panelRoot;
    private GameObject _toggleButton;

    // Tab buttons
    private Button _tab1Button;
    private Button _tab2Button;
    private int _activeTab = 0;

    // Content
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _stageText;
    private TextMeshProUGUI _descriptionText;

    private bool _isOpen = false;

    private readonly Color _tabActiveColor   = new Color(0.2f, 0.2f, 0.38f, 1f);
    private readonly Color _tabInactiveColor = new Color(0.1f, 0.1f, 0.18f, 1f);

    public static QuestUI Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        BuildUI();
    }

    void Update()
    {
        if (_toggleButton == null) return;

        bool anyStarted = QuestManager.Instance != null &&
            (QuestManager.Instance.quest1.IsStarted() ||
             QuestManager.Instance.quest2.IsStarted());

        _toggleButton.SetActive(anyStarted);
    }

    void BuildUI()
    {
        _canvasGO = new GameObject("QuestCanvas", typeof(RectTransform));
        Canvas canvas = _canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;
        _canvasGO.AddComponent<CanvasScaler>().uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _canvasGO.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(_canvasGO);

        BuildToggleButton(_canvasGO.transform);
        BuildPanel(_canvasGO.transform);

        _panelRoot.SetActive(false);
        _toggleButton.SetActive(false);
    }

    void BuildToggleButton(Transform parent)
    {
        _toggleButton = new GameObject("QuestButton", typeof(RectTransform));
        _toggleButton.transform.SetParent(parent, false);

        RectTransform rt = _toggleButton.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(1f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-12f, -12f);
        rt.sizeDelta        = new Vector2(120f, 38f);

        Image bg = _toggleButton.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.25f, 0.92f);

        Button btn = _toggleButton.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor      = bg.color;
        cb.highlightedColor = new Color(0.25f, 0.25f, 0.4f, 1f);
        cb.pressedColor     = new Color(0.1f, 0.1f, 0.18f, 1f);
        btn.colors          = cb;
        btn.onClick.AddListener(TogglePanel);

        MakeText(_toggleButton.transform, "📋 Quests", 15f, FontStyles.Normal,
            Color.white, Vector2.zero, Vector2.one,
            new Vector2(8f, 4f), new Vector2(-8f, -4f));
    }

    void BuildPanel(Transform parent)
    {
        _panelRoot = new GameObject("QuestPanel", typeof(RectTransform));
        _panelRoot.transform.SetParent(parent, false);

        RectTransform rt = _panelRoot.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(1f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-12f, -58f);
        rt.sizeDelta        = new Vector2(300f, 180f);

        _panelRoot.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.12f, 0.95f);

        // ── Tab row ────────────────────────────────────────────────────
        GameObject tabRow = new GameObject("TabRow", typeof(RectTransform));
        tabRow.transform.SetParent(_panelRoot.transform, false);
        RectTransform tabRT = tabRow.GetComponent<RectTransform>();
        tabRT.anchorMin        = new Vector2(0f, 1f);
        tabRT.anchorMax        = new Vector2(1f, 1f);
        tabRT.pivot            = new Vector2(0.5f, 1f);
        tabRT.anchoredPosition = Vector2.zero;
        tabRT.sizeDelta        = new Vector2(0f, 36f);

        HorizontalLayoutGroup hlg = tabRow.AddComponent<HorizontalLayoutGroup>();
        hlg.childControlWidth      = true;
        hlg.childControlHeight     = true;
        hlg.childForceExpandWidth  = true;
        hlg.childForceExpandHeight = true;
        hlg.spacing                = 2f;

        _tab1Button = MakeTabButton(tabRow.transform, "Quest 1", () => SelectTab(0));
        _tab2Button = MakeTabButton(tabRow.transform, "Quest 2", () => SelectTab(1));

        // ── Divider ────────────────────────────────────────────────────
        GameObject div = new GameObject("Divider", typeof(RectTransform));
        div.transform.SetParent(_panelRoot.transform, false);
        RectTransform divRT = div.GetComponent<RectTransform>();
        divRT.anchorMin        = new Vector2(0f, 1f);
        divRT.anchorMax        = new Vector2(1f, 1f);
        divRT.pivot            = new Vector2(0.5f, 1f);
        divRT.anchoredPosition = new Vector2(0f, -38f);
        divRT.sizeDelta        = new Vector2(0f, 1f);
        div.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.4f);

        // ── Content ────────────────────────────────────────────────────
        _titleText = MakeText(_panelRoot.transform, "Title", 16f, FontStyles.Bold,
            new Color(1f, 0.85f, 0.3f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(12f, -48f), new Vector2(-12f, -76f));

        _stageText = MakeText(_panelRoot.transform, "Stage", 11f, FontStyles.Normal,
            new Color(0.5f, 0.8f, 0.5f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(12f, -78f), new Vector2(-12f, -96f));

        _descriptionText = MakeText(_panelRoot.transform, "Description", 14f, FontStyles.Normal,
            Color.white,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(12f, -100f), new Vector2(-12f, -170f));
    }

    Button MakeTabButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(label, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        Image img = go.AddComponent<Image>();
        img.color = _tabInactiveColor;

        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor      = _tabInactiveColor;
        cb.highlightedColor = new Color(0.3f, 0.3f, 0.5f, 1f);
        cb.pressedColor     = new Color(0.08f, 0.08f, 0.15f, 1f);
        btn.colors          = cb;
        btn.onClick.AddListener(onClick);

        MakeText(go.transform, label, 13f, FontStyles.Normal, Color.white,
            Vector2.zero, Vector2.one, new Vector2(4f, 4f), new Vector2(-4f, -4f));

        return btn;
    }

    void SelectTab(int index)
    {
        _activeTab = index;
        RefreshPanel();

        // Highlight active tab
        _tab1Button.GetComponent<Image>().color = index == 0 ? _tabActiveColor : _tabInactiveColor;
        _tab2Button.GetComponent<Image>().color = index == 1 ? _tabActiveColor : _tabInactiveColor;
    }

    void TogglePanel()
    {
        _isOpen = !_isOpen;
        _panelRoot.SetActive(_isOpen);
        if (_isOpen) SelectTab(_activeTab);
    }

    void RefreshPanel()
    {
        if (QuestManager.Instance == null) return;

        QuestManager.QuestData quest = _activeTab == 0
            ? QuestManager.Instance.quest1
            : QuestManager.Instance.quest2;

        if (!quest.IsStarted())
        {
            _titleText.text       = "???";
            _stageText.text       = "";
            _descriptionText.text = "Not started yet.";
            return;
        }

        _titleText.text       = quest.GetTitle();
        _stageText.text       = quest.IsComplete()
            ? "✓ Complete"
            : $"Stage {quest.currentStage} of {quest.stages.Length}";
        _descriptionText.text = quest.GetDescription();
    }

    TextMeshProUGUI MakeText(Transform parent, string name, float size, FontStyles style,
        Color color, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot     = new Vector2(0f, 1f);
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text               = name;
        tmp.fontSize           = size;
        tmp.fontStyle          = style;
        tmp.color              = color;
        tmp.enableWordWrapping = true;
        return tmp;
    }
}