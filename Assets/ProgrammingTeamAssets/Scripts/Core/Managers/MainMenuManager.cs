using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private GameObject mainButtonsGroup;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject optionsButtonsGroup;
    [SerializeField] private Animator mainMenuAnimator;

    [Header("Transition")]
    [SerializeField] private Animator transition;

    private bool isLoading = false;

    private static readonly int HoverIndex = Animator.StringToHash("HoverIndex");

    public void Start()
    {
        mainButtonsGroup.SetActive(false);
        playButton.SetActive(false);
        optionsButtonsGroup.SetActive(false);
    }

    public void OnGameButtonPressed()
    {
        ClearImage();
        mainMenuAnimator.SetTrigger("GameButtonPressed");
    }

    public void OnOptionsButtonPressed()
    {
        ClearImage();
        mainMenuAnimator.SetTrigger("OptionsButtonPressed");
    }

    public void OnPlayButtonPressed()
    {
        if (isLoading) return;
        ClearImage();
        mainMenuAnimator.SetTrigger("PlayButtonPressed");
        StartCoroutine(PlayFadeOut());
    }

    private IEnumerator PlayFadeOut()
    {
        isLoading = true;

        GameStateManager.Instance?.ResetAllState();

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

        SceneManager.LoadScene(1);
    }

    public void OnExitOptionsButtonPressed()
    {
        ClearImage();
        mainMenuAnimator.SetTrigger("ExitOptionsButtonPressed");
    }

    public void ChangeImage(int buttonIndex)
    {
        if (mainMenuAnimator != null)
            mainMenuAnimator.SetInteger(HoverIndex, buttonIndex);
    }

    public void ClearImage()
    {
        if (mainMenuAnimator != null)
            mainMenuAnimator.SetInteger(HoverIndex, 0);
    }

    public void ExitGame()
    {
        Debug.Log("ExitGame called");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}