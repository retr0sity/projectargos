using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image previewImage;
    [SerializeField] private GameObject mainButtonsGroup;
    [SerializeField] private GameObject playButton;
    [SerializeField] private Animator mainMenuAnimator;

    private static readonly int HoverIndex = Animator.StringToHash("HoverIndex");

    public void Start()
    {
        mainButtonsGroup.SetActive(false);
        playButton.SetActive(false);
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
}
