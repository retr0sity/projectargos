using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Game.Managers;
using Game.Popups;
using Game.Controllers;
using Game.Components;

public class ConsoleMenuComponent : MonoBehaviour
{
    private InputManager InputManager => DependencyManager.Instance.InputManager;
    private InterfaceController InterfaceController => DependencyManager.Instance.InterfaceController;
    private ScreenFadeController ScreenFadeController => DependencyManager.Instance.ScreenFadeController;

    public Animator ConsoleAnimator;

    public GameObject YesNormal;
    public GameObject YesHighlight;
    public GameObject YesConfirmed;
    public GameObject NoNormal;

    private bool isYesSelected = true; // starts on Yes (left)
    private bool isConfirming = false;
    public GameObject YesOption;
    public GameObject NoOption;

    private PopupOkay mPopupOkay;
    private PopupOkay PopupOkay
    {
        get
        {
            if (mPopupOkay == null)
            {
                mPopupOkay = DependencyManager.Instance.PopupManager.GetPopup<PopupOkay>();
            }
            return mPopupOkay;
        }
    }
    public Sprite SpriteBasicControls;
    public void OpenMenu()
    {
        Debug.Log("OpenMenu called");
        gameObject.SetActive(true);
    }

    public void OnMenuOpened()
    {
        Debug.Log("OnMenuOpened called");
        ConsoleAnimator.SetTrigger("Open");
        YesOption.SetActive(true);
        NoOption.SetActive(true);

        YesHighlight.SetActive(true);
        YesNormal.SetActive(false);

        InputManager.InputButtonLeft.performed += OnLeftPressed;
        InputManager.InputButtonRight.performed += OnRightPressed;
        InputManager.InputButtonEnter.performed += OnEnterPressed;
    }

    private void OnRightPressed(InputAction.CallbackContext context)
    {
        if (isConfirming) return;
        if (!isYesSelected) return;

        // Move to No (just show normal yes sprite)
        AudioManager.Instance.Play("SelectionMove");
        isYesSelected = false;
        YesHighlight.SetActive(false);
        YesNormal.SetActive(true);
    }

    private void OnLeftPressed(InputAction.CallbackContext context)
    {
        if (isConfirming) return;
        if (isYesSelected) return;

        // Move to Yes
        AudioManager.Instance.Play("SelectionMove");
        isYesSelected = true;
        YesHighlight.SetActive(true);
        YesNormal.SetActive(false);
        NoNormal.SetActive(true);
    }


    private void OnEnterPressed(InputAction.CallbackContext context)
    {
        if (isConfirming) return;
        if (!isYesSelected) return;

        // Confirm Yes
        AudioManager.Instance.Play("Confirm");
        isConfirming = true;
        InputManager.InputButtonLeft.performed -= OnLeftPressed;
        InputManager.InputButtonRight.performed -= OnRightPressed;
        InputManager.InputButtonEnter.performed -= OnEnterPressed;

        YesHighlight.SetActive(false);
        YesConfirmed.SetActive(true);

        ConsoleAnimator.SetTrigger("Confirm");
    }

    // Call this via Animation Event on last frame of ConsoleMenuConfirmed
    public void OnConfirmFinished()
    {
        ScreenFadeController.FadeInAndOut(() =>
        {
            InterfaceController.ActionSwitchInterface?.Invoke("");
            PopupOkay.Show(SpriteBasicControls, () =>
            {
                FindObjectOfType<MainGameplayComponent>().ActionStartMainGameplay?.Invoke()
                ;
            });
        });
        
        //SceneManager.LoadScene("MainGame 1");
    }
}