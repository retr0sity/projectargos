using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Automatically loads a target scene after a set delay.
/// Attach to any GameObject in the elevator scene.
/// </summary>
public class TimedSceneTransition : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string targetSceneName = "";
    [SerializeField] private float delay = 7f;

    [Header("Transition")]
    [SerializeField] private Animator transition;

    void Start()
    {
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        yield return new WaitForSeconds(delay - 1f); // wait, leaving 1s for the fade

        if (transition != null)
        {
            transition.updateMode = AnimatorUpdateMode.UnscaledTime;
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(1f);
            transition.updateMode = AnimatorUpdateMode.Normal;
        }

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("[TimedSceneTransition] No target scene set!");
            yield break;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}