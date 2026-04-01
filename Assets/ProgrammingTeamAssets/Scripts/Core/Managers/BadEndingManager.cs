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
    [Tooltip("The name of the Animator component in each scene that handles the crossfade.")]
    [SerializeField] private string transitionObjectName = "Crossfade";

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

        if (InputManager.Instance != null)
            InputManager.Instance.SetMovementLock(true);

        // Find the crossfade animator in the CURRENT scene by name
        Animator transition = null;
        GameObject transitionGO = GameObject.Find(transitionObjectName);
        if (transitionGO != null)
            transition = transitionGO.GetComponentInChildren<Animator>();
        else
            Debug.LogWarning($"[BadEnding] Could not find '{transitionObjectName}' in current scene.");

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

        if (DialogueManager.Instance != null && monologueLines != null && monologueLines.Length > 0)
        {
            DialogueManager.Instance.StartMonologue(monologueLines);
            yield return new WaitUntil(() =>
                DialogueManager.Instance == null ||
                !DialogueManager.Instance.IsAnyUIActive()
            );
        }

        if (!string.IsNullOrEmpty(badEndingSceneName))
            SceneManager.LoadScene(badEndingSceneName);
    }

    public void ResetBadEnding()
    {
        hasTriggered = false;
    }
}