using UnityEngine;
using Game.Components;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Game.Utilities;
using Game.Managers;

public class DeskSelectionComponent : OptionSelectionComponent
{
    public List<DeskItemData> DeskItems;
    public Animator PaperAnimator;
    public TextComponent DescriptionText;
    public GameObject DescriptionPanel;
    private bool isPaperOpen = false;

    private void Start()
    {
        StartGameplay();
    }

    public new void StartGameplay()
    {
        Container.gameObject.SetActive(true);
        InputManager.InputButtonLeft.performed += OnLeftPressed;
        InputManager.InputButtonRight.performed += OnRightPressed;
        InputManager.InputButtonEnter.performed += OptionSelected;
        CurrentIndex = 0;
        ActionOptionChanged?.Invoke(DeskItems[CurrentIndex].ID);
    }

    protected override void OnLeftPressed(InputAction.CallbackContext context)
    {
        if (CurrentIndex == 0)
            return;

        CurrentIndex--;
        ActionOptionChanged?.Invoke(DeskItems[CurrentIndex].ID);
    }

    protected override void OnRightPressed(InputAction.CallbackContext context)
    {
        if (CurrentIndex == DeskItems.Count - 1)
            return;

        CurrentIndex++;
        ActionOptionChanged?.Invoke(DeskItems[CurrentIndex].ID);
    }

    protected override void OptionSelected(InputAction.CallbackContext context)
    {
        if (DeskItems[CurrentIndex].IsConsole)
        {
            SceneManager.LoadScene("MainGame 1");
            return;
        }

        if (!isPaperOpen)
        {
            DescriptionPanel.SetActive(true);
            DescriptionText.SetupText(DeskItems[CurrentIndex].Description);
            PaperAnimator.SetTrigger("Open");
            isPaperOpen = true;
        }
        else
        {
            PaperAnimator.SetTrigger("Close");
            isPaperOpen = false;
        }
    }
}