using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Managers;

/// <summary>
/// Persistent singleton that counts down the bad ending timer.
/// When it hits zero: locks movement, plays crossfade, shows monologue.
/// Triggered by SimpleNPCDialogue via GameStateManager.StartBadEndingTimer().
/// </summary>
public class BadEndingManager : MonoBehaviour
{
    public static BadEndingManager Instance { get; private set; }

    [Header("Bad Ending Monologue")]
    [TextArea(2, 4)]
    [SerializeField] private string[] monologueLines;

    [Header("Transition")]
    [SerializeField] private Animator transition;

    [Header("Bad Ending Scene (optional)")]
    [Tooltip("If set, loads this scene after the monologue. Leave empty to stay in current scene.")]
    [SerializeField] private string badEndingSceneName = "";

    private bool hasTriggered = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (hasTriggered) return;
        if (GameStateManager.Instance == null) return;
        if (!GameStateManager.Instance.badEndingTimerActive) return;

        GameStateManager.Instance.badEndingTimeRemaining -= Time.deltaTime;

        if (GameStateManager.Instance.badEndingTimeRemaining <= 0f)
        {
            GameStateManager.Instance.badEndingTimeRemaining = 0f;
            GameStateManager.Instance.badEndingTimerActive = false;
            hasTriggered = true;
            StartCoroutine(BadEndingSequence());
        }
    }

    private IEnumerator BadEndingSequence()
    {
        Debug.Log("[BadEnding] Timer hit zero — triggering bad ending.");

        // Lock movement, keep interact available so player can advance monologue
        if (InputManager.Instance != null)
            InputManager.Instance.SetMovementLock(true);

        // Play crossfade
        if (transition != null)
        {
            transition.updateMode = AnimatorUpdateMode.UnscaledTime;
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(1f);
            transition.updateMode = AnimatorUpdateMode.Normal;
        }
        else
        {
            yield return null;
        }

        // Play monologue and wait for it to finish
        if (DialogueManager.Instance != null && monologueLines != null && monologueLines.Length > 0)
        {
            DialogueManager.Instance.StartMonologue(monologueLines);

            // Wait until DialogueManager reports the monologue panel is no longer active
            yield return new WaitUntil(() =>
                DialogueManager.Instance == null ||
                !DialogueManager.Instance.IsAnyUIActive()
            );
        }

        // Load bad ending scene if set
        if (!string.IsNullOrEmpty(badEndingSceneName))
            SceneManager.LoadScene(badEndingSceneName);
    }

    public void ResetBadEnding()
    {
        hasTriggered = false;
    }
}