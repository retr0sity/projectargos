using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Game.Managers;

public class ConsoleMenuComponent : MonoBehaviour
{
    private InputManager InputManager => DependencyManager.Instance.InputManager;

    public Animator ConsoleAnimator;

    public GameObject YesNormal;
    public GameObject YesHighlight;
    public GameObject YesConfirmed;
    public GameObject NoNormal;

    private bool isYesSelected = true; // starts on Yes (left)
    private bool isConfirming = false;
    public GameObject YesOption;
    public GameObject NoOption;

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        // No trigger needed, ConsoleMenuOpen plays automatically on enable
    }

    // Call this via Animation Event on last frame of ConsoleMenuOpen
    public void OnMenuOpened()
    {
        YesOption.SetActive(true);

        // Start on Yes selected
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
        isYesSelected = false;
        YesHighlight.SetActive(false);
        YesNormal.SetActive(true);
    }

    private void OnLeftPressed(InputAction.CallbackContext context)
    {
        if (isConfirming) return;
        if (isYesSelected) return;

        // Move to Yes
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
        SceneManager.LoadScene("MainGame 1");
    }
}