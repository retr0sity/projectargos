using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;
    public Animator transition;


    private IEnumerator SceneFadeOut()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(1); // dramatic efe
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void TriggerSceneLoad() //care to put a fucking comment sto pou
    //to kaleis mhn sou gamhsw ton antixristo
    {
        StartCoroutine(SceneFadeOut());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SceneFadeOut());
        }
    }
}
