using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Always-visible hunger bar. Mirrors MoodHUD exactly — sits just below it in the top-left.
/// Attach to the same persistent GameObject as MoodHUD (or any DontDestroyOnLoad object).
/// Builds its own UI at runtime — no prefab needed.
/// </summary>
public class HungerHUD : MonoBehaviour
{
    [Header("UI References (optional — auto-created if null)")]
    public Slider hungerSlider;
    public TextMeshProUGUI hungerLabel;

    // How far below the mood bar to sit (pixels). Increase if bars overlap.
    [Header("Layout")]
    [SerializeField] private float verticalOffset = -54f;

    private Canvas _canvas;

    void Awake()
    {
        if (hungerSlider == null)
            BuildPlaceholderUI();
    }

    void Update()
    {
        if (HungerManager.Instance == null) return;

        float normalized = (HungerManager.Instance.hunger - HungerManager.Instance.minHunger)
                         / (HungerManager.Instance.maxHunger - HungerManager.Instance.minHunger);

        if (hungerSlider != null)
            hungerSlider.value = normalized;

        if (hungerLabel != null)
            hungerLabel.text = $"Hunger: {HungerManager.Instance.hunger:F0}";
    }

    void BuildPlaceholderUI()
    {
        // Reuse existing Canvas on this GO if MoodHUD already added one
        _canvas = gameObject.GetComponent<Canvas>();
        if (_canvas == null)
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 10;
            gameObject.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();
        }

        // Container panel — top-left, below mood bar
        GameObject panel = new GameObject("HungerPanel", typeof(RectTransform));
        panel.transform.SetParent(transform, false);
        RectTransform pt = panel.GetComponent<RectTransform>();
        pt.anchorMin        = new Vector2(0f, 1f);
        pt.anchorMax        = new Vector2(0f, 1f);
        pt.pivot            = new Vector2(0f, 1f);
        pt.anchoredPosition = new Vector2(16f, verticalOffset);
        pt.sizeDelta        = new Vector2(220f, 50f);

        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

        // Label
        GameObject labelGO = new GameObject("HungerLabel", typeof(RectTransform));
        labelGO.transform.SetParent(panel.transform, false);
        RectTransform lt = labelGO.GetComponent<RectTransform>();
        lt.anchorMin        = lt.anchorMax = new Vector2(0f, 1f);
        lt.pivot            = new Vector2(0f, 1f);
        lt.anchoredPosition = new Vector2(8f, -4f);
        lt.sizeDelta        = new Vector2(200f, 20f);

        hungerLabel = labelGO.AddComponent<TextMeshProUGUI>();
        hungerLabel.fontSize = 13f;
        hungerLabel.color    = Color.white;
        hungerLabel.text     = "Hunger: --";

        // Slider background track
        GameObject sliderGO = new GameObject("HungerSlider", typeof(RectTransform));
        sliderGO.transform.SetParent(panel.transform, false);
        RectTransform st = sliderGO.GetComponent<RectTransform>();
        st.anchorMin        = st.anchorMax = new Vector2(0f, 0f);
        st.pivot            = new Vector2(0f, 0f);
        st.anchoredPosition = new Vector2(8f, 6f);
        st.sizeDelta        = new Vector2(204f, 14f);

        hungerSlider             = sliderGO.AddComponent<Slider>();
        hungerSlider.minValue    = 0f;
        hungerSlider.maxValue    = 1f;
        hungerSlider.interactable = false;

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
        fillImg.color = new Color(0.9f, 0.65f, 0.2f, 1f);  // amber — distinct from mood's green

        hungerSlider.fillRect      = fr;
        hungerSlider.targetGraphic = trackImg;
    }
}