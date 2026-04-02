using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Hunger HUD that drives a frame-by-frame animation based on hunger percentage.
/// Assign an Animator with a single animation clip containing all hunger frames.
/// The script scrubs to the correct frame based on current hunger value.
/// </summary>
public class HungerHUD : MonoBehaviour
{
    [Header("Animation")]
    [Tooltip("Animator on the hunger UI image GameObject.")]
    [SerializeField] private Animator hungerAnimator;
    [Tooltip("Exact name of the animation state in the Animator.")]
    [SerializeField] private string animationStateName = "HungerAnimation";

    [Header("UI References (optional — auto-created if null)")]
    public TextMeshProUGUI hungerLabel;

    [Header("Layout")]
    [SerializeField] private float verticalOffset = -54f;

    private Canvas _canvas;
    private bool _animatorReady = false;

    void Start()
    {
        if (hungerLabel == null)
            BuildPlaceholderUI();

        if (hungerAnimator != null)
        {
            hungerAnimator.speed = 0f;
            _animatorReady = true;
        }
        else
        {
            Debug.LogWarning("[HungerHUD] No Animator assigned — animation won't play.");
        }
    }

    void Update()
    {
        if (HungerManager.Instance == null) return;

        float normalized = (HungerManager.Instance.hunger - HungerManager.Instance.minHunger)
                         / (HungerManager.Instance.maxHunger - HungerManager.Instance.minHunger);

        // Scrub animator to correct frame based on hunger
        if (_animatorReady && hungerAnimator != null)
        {
            // Invert: full hunger = first frame, empty = last frame
            float animPosition = normalized;
            hungerAnimator.Play(animationStateName, 0, animPosition);
        }

    }

    void BuildPlaceholderUI()
    {
        // Reuse MoodHUD's canvas — don't create a new one
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

        // Panel sits directly below MoodHUD panel (-16 offset - 50 height = -66)
        GameObject panel = new GameObject("HungerPanel", typeof(RectTransform));
        panel.transform.SetParent(transform, false);
        RectTransform pt = panel.GetComponent<RectTransform>();
        pt.anchorMin        = new Vector2(0f, 1f);
        pt.anchorMax        = new Vector2(0f, 1f);
        pt.pivot            = new Vector2(0f, 1f);
        pt.anchoredPosition = new Vector2(16f, -66f);
        pt.sizeDelta        = new Vector2(220f, 50f);

        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
    }
}