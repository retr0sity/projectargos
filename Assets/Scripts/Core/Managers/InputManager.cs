using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Managers
{
    /// <summary>
    /// Centralized Input Manager using Unity's new Input System and generated PlayerControls.
    /// Emits high-level input events without handling gameplay logic directly.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class InputManager : MonoBehaviour,
        PlayerControls.IGlobalActions,
        PlayerControls.IGameplayActions,
        PlayerControls.IUIActions
    {
        public static InputManager Instance { get; private set; }

        // Public events for external systems to subscribe
        public event Action<Vector2> MoveEvent;
        public event Action JumpEvent;
        public event Action LandingEvent;
        public event Action InteractEvent;
        public event Action InventoryEvent;
        public event Action PauseEvent;

        public event Action<Vector2> NavigateEvent;
        public event Action SubmitEvent;
        public event Action<Vector2> ScrollEvent;
        public event Action<Vector2> PointEvent;

        private PlayerControls _controls;
        private bool _controlsLocked;

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
            _controls.Global.SetCallbacks(this);
            _controls.Gameplay.SetCallbacks(this);
            _controls.UI.SetCallbacks(this);

            _controls.Global.Enable();
            _controls.Gameplay.Enable();
            _controls.UI.Enable();
        }

        void OnDestroy()
        {
            _controls.Global.RemoveCallbacks(this);
            _controls.Gameplay.RemoveCallbacks(this);
            _controls.UI.RemoveCallbacks(this);
            _controls.Dispose();
        }

        /// <summary>
        /// Enable or disable input handling (e.g. during cutscenes).
        /// </summary>
        public void SetControlLock(bool locked)
        {
            _controlsLocked = locked;
            if (locked)
                _controls.Gameplay.Disable();
            else
                _controls.Gameplay.Enable();
        }

        // IGlobalActions
        void PlayerControls.IGlobalActions.OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
                PauseEvent?.Invoke();
        }

        // IGameplayActions
        void PlayerControls.IGameplayActions.OnMove(InputAction.CallbackContext context)
        {
            if (_controlsLocked) return;
            if (context.performed || context.canceled)
                MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        void PlayerControls.IGameplayActions.OnJump(InputAction.CallbackContext context)
        {
            if (_controlsLocked) return;
            if (context.performed)
                JumpEvent?.Invoke();
        }

        void PlayerControls.IGameplayActions.OnInteract(InputAction.CallbackContext context)
        {
            if (_controlsLocked) return;
            if (context.performed)
                InteractEvent?.Invoke();
        }

        void PlayerControls.IGameplayActions.OnInventory(InputAction.CallbackContext context)
        {
            if (_controlsLocked) return;
            if (context.performed)
                InventoryEvent?.Invoke();
        }

        // IUIActions
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

        /// <summary>
        /// Should be called by gameplay systems when player lands to reset jump state.
        /// </summary>
        public void TriggerLanding() => LandingEvent?.Invoke();
    }
}