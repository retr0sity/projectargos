using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private GameObject mainButtonsGroup;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject optionsButtonsGroup;
    [SerializeField] private Animator mainMenuAnimator;

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
        ClearImage();
        mainMenuAnimator.SetTrigger("PlayButtonPressed");
        GameStateManager.Instance?.ResetAllState();
        SceneManager.LoadScene(1);
    }

    public void OnExitOptionsButtonPressed()
    {
        ClearImage();
        mainMenuAnimator.SetTrigger("ExitOptionsButtonPressed");
    }


    // Called when hovering a button
    public void ChangeImage(int buttonIndex)
    {
        if (mainMenuAnimator != null)
            mainMenuAnimator.SetInteger(HoverIndex, buttonIndex);
    }

    // Called when exiting a button
    public void ClearImage()
    {
        if (mainMenuAnimator != null)
            mainMenuAnimator.SetInteger(HoverIndex, 0);
    }

    public void ExitGame()
    {
        Debug.Log("ExitGame called"); // Just to confirm it works in Editor

        // Closes the application
        Application.Quit();

        // If you're in the Unity Editor, this won't do anything.
        // You can add this line just for testing:
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
