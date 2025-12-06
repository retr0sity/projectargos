using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Managers
{
    [DefaultExecutionOrder(-100)]
    public class InputManager : MonoBehaviour,
        PlayerControls.IGlobalActions,
        PlayerControls.IGameplayActions, // This now requires OnRun to be implemented
        PlayerControls.IUIActions
    {
        public static InputManager Instance { get; private set; }

        // --- Events ---
        public event Action<Vector2> MoveEvent;
        public event Action JumpEvent;
        public event Action<bool> RunEvent; // <--- NEW RUN EVENT
        public event Action LandingEvent;
        public event Action InteractEvent;
        public event Action InventoryEvent;
        public event Action PauseEvent;
        
        // Locking Events
        public event Action<bool> ControlLockChanged;
        private bool _movementLocked;
        public event Action<bool> MovementLockChanged;

        // UI Events
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
            if (_controls != null) { _controls.Disable(); _controls.Dispose(); }
        }

        public void SetControlLock(bool locked)
        {
            _controlsLocked = locked;
            if (locked) _controls.Gameplay.Disable();
            else _controls.Gameplay.Enable();

            ControlLockChanged?.Invoke(locked);
            
            if (!locked && !_movementLocked) MoveEvent?.Invoke(Vector2.zero);
        }

        public void SetMovementLock(bool locked)
        {
            _movementLocked = locked;
            MovementLockChanged?.Invoke(locked);
            
            if (!locked && !_controlsLocked) MoveEvent?.Invoke(Vector2.zero);
        }

        public bool IsMovementLocked() => _controlsLocked || _movementLocked;

        // --- Gameplay Actions ---

        void PlayerControls.IGameplayActions.OnMove(InputAction.CallbackContext context)
        {
            if (_controlsLocked || _movementLocked) return;
            if (context.performed || context.canceled)
                MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        void PlayerControls.IGameplayActions.OnJump(InputAction.CallbackContext context)
        {
            if (_controlsLocked) return;
            if (context.performed) JumpEvent?.Invoke();
        }

        // <--- NEW: RUN IMPLEMENTATION --->
        void PlayerControls.IGameplayActions.OnRun(InputAction.CallbackContext context)
        {
            if (_controlsLocked) return;

            if (context.performed)
                RunEvent?.Invoke(true); // Key Pressed
            else if (context.canceled)
                RunEvent?.Invoke(false); // Key Released
        }

        void PlayerControls.IGameplayActions.OnInteract(InputAction.CallbackContext context)
        {
            if (_controlsLocked) return;
            if (context.performed) InteractEvent?.Invoke();
        }

        void PlayerControls.IGameplayActions.OnInventory(InputAction.CallbackContext context)
        {
            if (_controlsLocked) return;
            if (context.performed) InventoryEvent?.Invoke();
        }

        // --- UI & Global Actions (Standard Implementation) ---
        void PlayerControls.IGlobalActions.OnPause(InputAction.CallbackContext context) { if (context.performed) PauseEvent?.Invoke(); }
        void PlayerControls.IUIActions.OnNavigation(InputAction.CallbackContext context) { if (context.performed || context.canceled) NavigateEvent?.Invoke(context.ReadValue<Vector2>()); }
        void PlayerControls.IUIActions.OnSubmit(InputAction.CallbackContext context) { if (context.performed) SubmitEvent?.Invoke(); }
        void PlayerControls.IUIActions.OnScroll(InputAction.CallbackContext context) { if (context.performed) ScrollEvent?.Invoke(context.ReadValue<Vector2>()); }
        void PlayerControls.IUIActions.OnPoint(InputAction.CallbackContext context) { if (context.performed) PointEvent?.Invoke(context.ReadValue<Vector2>()); }

        public void TriggerLanding() => LandingEvent?.Invoke();
        public InputAction GetMoveAction() => _controls?.Gameplay.Move;
    }
}