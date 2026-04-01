using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Managers;

/// <summary>
/// Displays the bad ending countdown timer as MM:SS at the top center of the screen.
/// Attach to any persistent GameObject alongside BadEndingManager.
/// Builds its own UI at runtime — no prefab needed.
/// </summary>
public class BadEndingTimerUI : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color urgentColor = new Color(1f, 0.25f, 0.25f); // red
    [Tooltip("When time remaining drops below this (seconds), color switches to urgent.")]
    [SerializeField] private float urgentThreshold = 60f;

    private TextMeshProUGUI _timerText;
    private GameObject _panelRoot;

    public static BadEndingTimerUI Instance { get; private set; }

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
        if (_timerText == null) return;

        if (GameStateManager.Instance == null || !GameStateManager.Instance.badEndingTimerActive)
        {
            _panelRoot.SetActive(false);
            return;
        }

        _panelRoot.SetActive(true);

        float remaining = Mathf.Max(0f, GameStateManager.Instance.badEndingTimeRemaining);
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        _timerText.text = $"{minutes:00}:{seconds:00}";
        _timerText.color = remaining <= urgentThreshold ? urgentColor : normalColor;
    }

    void BuildUI()
    {
        // Find or create a canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("TimerCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Root panel — top center anchor
        _panelRoot = new GameObject("BadEndingTimerPanel", typeof(RectTransform));
        _panelRoot.transform.SetParent(canvas.transform, false);

        RectTransform rt = _panelRoot.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 1f);
        rt.anchorMax        = new Vector2(0.5f, 1f);
        rt.pivot            = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -12f);
        rt.sizeDelta        = new Vector2(140f, 48f);

        // Background
        Image bg = _panelRoot.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.55f);

        // Timer text
        GameObject textGO = new GameObject("TimerText", typeof(RectTransform));
        textGO.transform.SetParent(_panelRoot.transform, false);

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin        = Vector2.zero;
        textRT.anchorMax        = Vector2.one;
        textRT.offsetMin        = new Vector2(8f, 4f);
        textRT.offsetMax        = new Vector2(-8f, -4f);

        _timerText = textGO.AddComponent<TextMeshProUGUI>();
        _timerText.text      = "00:00";
        _timerText.fontSize  = 26f;
        _timerText.fontStyle = FontStyles.Bold;
        _timerText.color     = normalColor;
        _timerText.alignment = TextAlignmentOptions.Center;

        // Hidden until timer is active
        _panelRoot.SetActive(false);
    }
}