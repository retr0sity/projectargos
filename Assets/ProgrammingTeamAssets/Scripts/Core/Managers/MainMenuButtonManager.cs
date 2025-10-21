using UnityEngine;

public class MainMenuButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject mainButtonsGroup;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject optionsButtonsGroup;

    // Called by animation event
    public void ShowMainButtonsGroup()
    {
        mainButtonsGroup.SetActive(true);
    }

    public void HideMainButtonsGroup()
    {
        mainButtonsGroup.SetActive(false);
    }

    public void ShowPlayButton()
    {
        playButton.SetActive(true);
    }

    public void ShowOptionsButtonsGroup()
    {
        optionsButtonsGroup.SetActive(true);
    }

    public void HideOptionsButtonsGroup()
    {
        optionsButtonsGroup.SetActive(false);
    }

}
