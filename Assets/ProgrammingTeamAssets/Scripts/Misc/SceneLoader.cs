using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    private const string MainMenuSceneName = "00_Main Menu 3.0";

    public string sceneName;
    public Animator transition;
    private bool isLoading;


    private IEnumerator SceneFadeOut()
    {
        if (isLoading) yield break;
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"SceneLoader on '{gameObject.name}' has no sceneName configured.");
            yield break;
        }

        isLoading = true;

        if (transition != null)
        {
            transition.updateMode = AnimatorUpdateMode.UnscaledTime;
            transition.SetTrigger("Start");
            yield return new WaitForSecondsRealtime(1); // dramatic efe
            transition.updateMode = AnimatorUpdateMode.Normal;
        }
        else
        {
            yield return null;
        }

        if (sceneName == MainMenuSceneName)
            GameStateManager.Instance?.ResetAllState();

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void TriggerSceneLoad() //care to put a fucking comment sto pou
    //to kaleis mhn sou gamhsw ton antixristo
    {
        if (!isLoading)
            StartCoroutine(SceneFadeOut());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isLoading)
                StartCoroutine(SceneFadeOut());
        }
    }
}
