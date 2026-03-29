using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Always-visible mood bar in the corner of the screen.
/// Attach to a Canvas > Panel > (Bar + Label) hierarchy.
/// All UI is created in code — no prefab needed for placeholder.
/// </summary>
public class MoodHUD : MonoBehaviour
{
    // ── References (assign in Inspector OR let OnEnable create them) ──
    [Header("UI References (optional — auto-created if null)")]
    public Slider moodSlider;
    public TextMeshProUGUI moodLabel;

    // ── Runtime ──
    private Canvas _canvas;

    void Awake()
    {
        // If the designer hasn't wired anything up yet, build a minimal UI ourselves.
        if (moodSlider == null)
            BuildPlaceholderUI();
    }

    void Update()
    {
        if (MoodManager.Instance == null) return;

        float normalized = (MoodManager.Instance.mood - MoodManager.Instance.minMood)
                         / (MoodManager.Instance.maxMood - MoodManager.Instance.minMood);

        if (moodSlider != null)
            moodSlider.value = normalized;

        if (moodLabel != null)
            moodLabel.text = $"Mood: {MoodManager.Instance.mood:F0}";
    }

    // ─────────────────────────────────────────────────────────────────
    // Placeholder builder — produces a clean bar in the top-left corner.
    // Delete this whole region once artists hand off real assets.
    // ─────────────────────────────────────────────────────────────────
    void BuildPlaceholderUI()
    {
       // Replace the three AddComponent lines at the top of BuildPlaceholderUI with:
        _canvas = gameObject.GetComponent<Canvas>();
        if (_canvas == null)
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 10;
        }

        if (gameObject.GetComponent<CanvasScaler>() == null)
            gameObject.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

        if (gameObject.GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        // Container panel — top-left
        GameObject panel = new GameObject("MoodPanel", typeof(RectTransform));
        panel.transform.SetParent(transform, false);
        RectTransform pt = panel.GetComponent<RectTransform>();
        pt.anchorMin = new Vector2(0f, 1f);
        pt.anchorMax = new Vector2(0f, 1f);
        pt.pivot     = new Vector2(0f, 1f);
        pt.anchoredPosition = new Vector2(16f, -16f);
        pt.sizeDelta = new Vector2(220f, 50f);

        // Dark background
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.55f);

        // Label
        GameObject labelGO = new GameObject("MoodLabel", typeof(RectTransform));
        labelGO.transform.SetParent(panel.transform, false);
        RectTransform lt = labelGO.GetComponent<RectTransform>();
        lt.anchorMin = lt.anchorMax = new Vector2(0f, 1f);
        lt.pivot = new Vector2(0f, 1f);
        lt.anchoredPosition = new Vector2(8f, -4f);
        lt.sizeDelta = new Vector2(200f, 20f);

        moodLabel = labelGO.AddComponent<TextMeshProUGUI>();
        moodLabel.fontSize = 13f;
        moodLabel.color = Color.white;
        moodLabel.text = "Mood: --";

        // Slider background track
        GameObject sliderGO = new GameObject("MoodSlider", typeof(RectTransform));
        sliderGO.transform.SetParent(panel.transform, false);
        RectTransform st = sliderGO.GetComponent<RectTransform>();
        st.anchorMin = st.anchorMax = new Vector2(0f, 0f);
        st.pivot = new Vector2(0f, 0f);
        st.anchoredPosition = new Vector2(8f, 6f);
        st.sizeDelta = new Vector2(204f, 14f);

        moodSlider = sliderGO.AddComponent<Slider>();
        moodSlider.minValue = 0f;
        moodSlider.maxValue = 1f;
        moodSlider.interactable = false;   // display only

        // Track image
        Image trackImg = sliderGO.AddComponent<Image>();
        trackImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        // Fill area
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderGO.transform, false);
        RectTransform fa = fillArea.GetComponent<RectTransform>();
        fa.anchorMin = Vector2.zero;
        fa.anchorMax = Vector2.one;
        fa.offsetMin = fa.offsetMax = Vector2.zero;

        // Fill
        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fr = fill.GetComponent<RectTransform>();
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = Vector2.one;
        fr.offsetMin = fr.offsetMax = Vector2.zero;

        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.3f, 0.85f, 0.45f, 1f);  // green fill

        moodSlider.fillRect = fr;
        moodSlider.targetGraphic = trackImg;
    }
}
