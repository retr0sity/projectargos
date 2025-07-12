using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Core.Managers;

/// <summary>
/// Input manager for the Arduino minigame, plus global/UI handling.
/// </summary>
[DefaultExecutionOrder(-100)]
public class ArduinoInputManager : MonoBehaviour,
    PlayerControls.IGlobalActions,
    PlayerControls.IUIActions,
    PlayerControls.IMinigame_ArduinoActions
{
    public static ArduinoInputManager Instance { get; private set; }

    // Global/UI events
    public event Action PauseEvent;
    public event Action<Vector2> NavigateEvent;
    public event Action SubmitEvent;
    public event Action<Vector2> ScrollEvent;
    public event Action<Vector2> PointEvent;

    // Minigame_Arduino events
    public event Action ArduinoJumpEvent;

    private PlayerControls _controls;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _controls = new PlayerControls();

        // Register callbacks
        _controls.Global.SetCallbacks(this);
        _controls.UI.SetCallbacks(this);
        _controls.Minigame_Arduino.SetCallbacks(this);

        // Enable needed maps
        _controls.Global.Enable();
        _controls.UI.Enable();
        _controls.Minigame_Arduino.Enable();
    }

    void OnDestroy()
    {
        // Remove callbacks and dispose
        _controls.Global.RemoveCallbacks(this);
        _controls.UI.RemoveCallbacks(this);
        _controls.Minigame_Arduino.RemoveCallbacks(this);
        _controls.Dispose();
    }

    // --- IGlobalActions ---
    void PlayerControls.IGlobalActions.OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
            PauseEvent?.Invoke();
    }

    // --- IUIActions ---
    void PlayerControls.IUIActions.OnNavigation(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
            NavigateEvent?.Invoke(context.ReadValue<Vector2>());
    }

    void PlayerControls.IUIActions.OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
            SubmitEvent?.Invoke();
    }

    void PlayerControls.IUIActions.OnScroll(InputAction.CallbackContext context)
    {
        if (context.performed)
            ScrollEvent?.Invoke(context.ReadValue<Vector2>());
    }

    void PlayerControls.IUIActions.OnPoint(InputAction.CallbackContext context)
    {
        if (context.performed)
            PointEvent?.Invoke(context.ReadValue<Vector2>());
    }

    // --- IMinigame_ArduinoActions ---
    void PlayerControls.IMinigame_ArduinoActions.OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            ArduinoJumpEvent?.Invoke();
    }
}
