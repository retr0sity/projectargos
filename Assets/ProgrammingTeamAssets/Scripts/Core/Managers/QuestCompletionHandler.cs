using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Managers;

/// <summary>
/// Watches for quest completion and immediately shows a yes/no choice.
/// Stops the bad ending timer, plays crossfade, then monologue before loading the outcome scene.
/// Attach to any persistent GameObject.
/// </summary>
public class QuestCompletionHandler : MonoBehaviour
{
    [Header("Choice")]
    [SerializeField] private string choicePrompt = "Do you want to go to Demetra's party?";

    [Header("Yes Outcome")]
    [SerializeField] private string yesSceneName = "scene1";
    [TextArea(2, 4)]
    [SerializeField] private string[] yesMonologue = { "You decide to go to the party." };

    [Header("No Outcome")]
    [SerializeField] private string noSceneName = "scene2";
    [TextArea(2, 4)]
    [SerializeField] private string[] noMonologue = { "You decide to stay home." };

    [Header("Transition")]
    [Tooltip("Name of the crossfade GameObject in each scene.")]
    [SerializeField] private string transitionObjectName = "Crossfade";

    private bool hasTriggered = false;
    private int lastCheckedStage = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (hasTriggered) return;
        if (QuestManager.Instance == null) return;

        // Watch for quest completion
        if (!QuestManager.Instance.IsComplete()) return;
        if (QuestManager.Instance.currentStage == lastCheckedStage) return;

        hasTriggered = true;
        lastCheckedStage = QuestManager.Instance.currentStage;
        StartCoroutine(ShowChoiceAfterFrame());
    }

    // Wait one frame so inventory/quest state fully settles before showing choice
    private IEnumerator ShowChoiceAfterFrame()
    {
        yield return null;

        // Stop the bad ending timer
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.StopBadEndingTimer();

        // Show the yes/no choice
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowChoices(
                new[] { "Yes", "No" },
                OnChoiceMade,
                choicePrompt
            );
        }
    }

    private void OnChoiceMade(int index)
    {
        if (index == 0)
            StartCoroutine(OutcomeSequence(yesMonologue, yesSceneName));
        else
            StartCoroutine(OutcomeSequence(noMonologue, noSceneName));
    }

    private IEnumerator OutcomeSequence(string[] monologue, string sceneName)
    {
        // Lock movement, keep interact for advancing monologue
        if (InputManager.Instance != null)
            InputManager.Instance.SetMovementLock(true);

        // Find crossfade in current scene
        Animator transition = null;
        GameObject transitionGO = GameObject.Find(transitionObjectName);
        if (transitionGO != null)
            transition = transitionGO.GetComponentInChildren<Animator>();
        else
            Debug.LogWarning($"[QuestCompletion] Could not find '{transitionObjectName}' in scene.");

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
        if (DialogueManager.Instance != null && monologue != null && monologue.Length > 0)
        {
            DialogueManager.Instance.StartMonologue(monologue);

            yield return new WaitUntil(() =>
                DialogueManager.Instance == null ||
                !DialogueManager.Instance.IsAnyUIActive()
            );
        }

        // Load outcome scene
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }

    public void Reset()
    {
        hasTriggered = false;
        lastCheckedStage = 0;
    }
}