using UnityEngine;
using Game.Components;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Game.Utilities;
using Game.Managers;
using System.Collections;

public class DeskSelectionComponent : OptionSelectionComponent
{
    public List<DeskItemData> DeskItems;
    public Animator PaperAnimator;
    public TextComponent DescriptionText;
    public GameObject DescriptionPanel;
    private bool isPaperOpen = false;
    public ConsoleMenuComponent ConsoleMenu;

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
        AudioManager.Instance.Play("SelectionMove"); // add this
    }

    protected override void OnRightPressed(InputAction.CallbackContext context)
    {
        if (CurrentIndex == DeskItems.Count - 1)
            return;

        CurrentIndex++;
        ActionOptionChanged?.Invoke(DeskItems[CurrentIndex].ID);
        AudioManager.Instance.Play("SelectionMove"); // add this
    }

    protected override void OptionSelected(InputAction.CallbackContext context)
    {
        if (DeskItems[CurrentIndex].IsConsole)
        {
            ConsoleMenu.OpenMenu();
            return;
        }

        // Unsubscribe immediately to prevent repeated triggers
        InputManager.InputButtonEnter.performed -= OptionSelected;

        if (!isPaperOpen)
        {
            AudioManager.Instance.Play("PaperOpen");
            DescriptionPanel.SetActive(true);
            DescriptionText.SetupText(DeskItems[CurrentIndex].Description);
            PaperAnimator.SetTrigger("Open");
            isPaperOpen = true;
        }
        else
        {
            AudioManager.Instance.Play("PaperOpen");
            PaperAnimator.SetTrigger("Close");
            isPaperOpen = false;
        }

        // Resubscribe after a short delay
        StartCoroutine(ResubscribeEnter());
    }

    private IEnumerator ResubscribeEnter()
    {
        yield return new WaitForSeconds(0.5f);
        InputManager.InputButtonEnter.performed += OptionSelected;
    }
}