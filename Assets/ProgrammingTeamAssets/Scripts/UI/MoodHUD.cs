using UnityEngine;

/// <summary>
/// Mood HUD that drives a frame-by-frame animation based on mood percentage.
/// Assign an Animator with a single animation clip containing all mood frames.
/// The script scrubs to the correct frame based on current mood value.
/// </summary>
public class MoodHUD : MonoBehaviour
{
    [Header("Animation")]
    [Tooltip("Animator on the mood UI image GameObject.")]
    [SerializeField] private Animator moodAnimator;

    [Tooltip("Exact name of the animation state in the Animator.")]
    [SerializeField] private string animationStateName = "MoodAnimation";

    private bool _animatorReady = false;

    void Start()
    {
        if (moodAnimator != null)
        {
            moodAnimator.speed = 0f;
            _animatorReady = true;
        }
        else
        {
            Debug.LogWarning("[MoodHUD] No Animator assigned — animation won't play.");
        }
    }

    void Update()
    {
        if (MoodManager.Instance == null) return;

        float normalized = (MoodManager.Instance.mood - MoodManager.Instance.minMood)
                         / (MoodManager.Instance.maxMood - MoodManager.Instance.minMood);

        normalized = Mathf.Clamp01(normalized);

        if (_animatorReady && moodAnimator != null)
        {
            moodAnimator.Play(animationStateName, 0, normalized);
        }
    }
}