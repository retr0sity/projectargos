using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Persistent quest tracker UI. Toggle button sits top-right in every scene.
/// Attach to any persistent GameObject (e.g. alongside GameStateManager).
/// </summary>
public class QuestUI : MonoBehaviour
{
    private GameObject _canvasGO;
    private GameObject _panelRoot;
    private GameObject _toggleButton;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _descriptionText;
    private TextMeshProUGUI _stageText;
    private bool _isOpen = false;

    void Start()
    {
        BuildUI();
    }

    void Update()
    {
        if (_toggleButton == null) return;

        // Hide button entirely if quest hasn't started
        _toggleButton.SetActive(QuestManager.Instance != null && QuestManager.Instance.currentStage > 0);
    }

    void BuildUI()
    {
        // Dedicated canvas so it sits above everything
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
        cb.highlightedColor = new Color(0.25f, 0.25f, 0.4f, 1f);
        cb.pressedColor     = new Color(0.1f, 0.1f, 0.18f, 1f);
        btn.colors          = cb;
        btn.onClick.AddListener(TogglePanel);

        GameObject labelGO = new GameObject("Label", typeof(RectTransform));
        labelGO.transform.SetParent(_toggleButton.transform, false);
        RectTransform labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = Vector2.zero;
        labelRT.anchorMax = Vector2.one;
        labelRT.offsetMin = new Vector2(8f, 4f);
        labelRT.offsetMax = new Vector2(-8f, -4f);

        TextMeshProUGUI label = labelGO.AddComponent<TextMeshProUGUI>();
        label.text      = "📋 Quest";
        label.fontSize  = 15f;
        label.color     = Color.white;
        label.alignment = TextAlignmentOptions.Center;
    }

    void BuildPanel(Transform parent)
    {
        _panelRoot = new GameObject("QuestPanel", typeof(RectTransform));
        _panelRoot.transform.SetParent(parent, false);

        RectTransform rt = _panelRoot.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(1f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-12f, -58f); // just below the button
        rt.sizeDelta        = new Vector2(280f, 140f);

        Image bg = _panelRoot.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.12f, 0.95f);

        // Quest title
        _titleText = MakeText(_panelRoot.transform, "Quest Title", 16f, FontStyles.Bold,
            new Color(1f, 0.85f, 0.3f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(12f, -12f), new Vector2(280f, 26f));

        // Divider
        GameObject div = new GameObject("Divider", typeof(RectTransform));
        div.transform.SetParent(_panelRoot.transform, false);
        RectTransform divRT = div.GetComponent<RectTransform>();
        divRT.anchorMin        = new Vector2(0f, 1f);
        divRT.anchorMax        = new Vector2(1f, 1f);
        divRT.pivot            = new Vector2(0.5f, 1f);
        divRT.anchoredPosition = new Vector2(0f, -42f);
        divRT.sizeDelta        = new Vector2(-16f, 1f);
        div.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.4f);

        // Stage indicator
        _stageText = MakeText(_panelRoot.transform, "Stage", 11f, FontStyles.Normal,
            new Color(0.5f, 0.8f, 0.5f),
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(12f, -48f), new Vector2(280f, 18f));

        // Description
        _descriptionText = MakeText(_panelRoot.transform, "Description", 14f, FontStyles.Normal,
            Color.white,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(12f, -68f), new Vector2(280f, 60f));
    }

    void TogglePanel()
    {
        _isOpen = !_isOpen;
        _panelRoot.SetActive(_isOpen);

        if (_isOpen)
            RefreshPanel();
    }

    void RefreshPanel()
    {
        if (QuestManager.Instance == null) return;

        _titleText.text       = QuestManager.Instance.GetTitle();
        _stageText.text       = QuestManager.Instance.IsComplete()
            ? "✓ Complete"
            : $"Stage {QuestManager.Instance.currentStage} of {QuestManager.Instance.stages.Length}";
        _descriptionText.text = QuestManager.Instance.GetDescription();
    }

    TextMeshProUGUI MakeText(Transform parent, string name, float size, FontStyles style,
        Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = new Vector2(0f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = sizeDelta;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text               = name;
        tmp.fontSize           = size;
        tmp.fontStyle          = style;
        tmp.color              = color;
        tmp.enableWordWrapping = true;
        return tmp;
    }
}