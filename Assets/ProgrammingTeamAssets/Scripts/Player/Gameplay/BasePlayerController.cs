using UnityEngine;
using Core.Managers;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public abstract class BasePlayerController : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float baseSpeed = 5f;
    [SerializeField] protected bool lockAxisX = false;
    [SerializeField] protected bool lockAxisY = false;

    protected Rigidbody2D _rb;
    protected Animator _animator;
    
    // State
    protected Vector2 _currentInput;
    protected bool _isRunningInput;
    protected bool _controlsLocked = false;
    protected bool _isMovementLocked = false;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        var input = InputManager.Instance;
        if (input != null)
        {
            input.MoveEvent += OnMoveInput;
            input.JumpEvent += OnJumpInput;
            input.RunEvent += OnRunInput; // Subscribe to Run
            input.ControlLockChanged += OnControlLockChanged;
            input.MovementLockChanged += OnMovementLockChanged;

            // Pick up current lock state in case the controller is enabled while UI is already active.
            _controlsLocked = input.IsControlLocked();
            _isMovementLocked = input.IsMovementOnlyLocked();
            if (_controlsLocked || _isMovementLocked) StopMovement();
        }
    }

    protected virtual void OnDisable()
    {
        var input = InputManager.Instance;
        if (input != null)
        {
            input.MoveEvent -= OnMoveInput;
            input.JumpEvent -= OnJumpInput;
            input.RunEvent -= OnRunInput;
            input.ControlLockChanged -= OnControlLockChanged;
            input.MovementLockChanged -= OnMovementLockChanged;
        }
        StopMovement();
    }

    // --- Input Handlers ---

    private void OnMoveInput(Vector2 input)
    {
        if (_controlsLocked || _isMovementLocked)
        {
            _currentInput = Vector2.zero;
            return;
        }
        _currentInput = input;
    }

    private void OnJumpInput()
    {
        if (_controlsLocked || _isMovementLocked) return;
        HandleJump();
    }

    private void OnRunInput(bool isRunning)
    {
        if (_controlsLocked || _isMovementLocked)
        {
            _isRunningInput = false;
            return;
        }
        _isRunningInput = isRunning;
    }

    // --- Locking Logic ---

    protected virtual void OnControlLockChanged(bool isLocked)
    {
        _controlsLocked = isLocked;
        if (isLocked) StopMovement();
    }

    protected virtual void OnMovementLockChanged(bool isLocked)
    {
        _isMovementLocked = isLocked;
        if (isLocked) StopMovement();
    }

    protected virtual void StopMovement()
    {
        _currentInput = Vector2.zero;
        _isRunningInput = false;
        if(_rb != null) _rb.linearVelocity = Vector2.zero;
        if(_animator != null) _animator.SetFloat("Speed", 0f);
    }

    public void ForceStop() => StopMovement();

    // --- Physics Loop ---

    protected virtual void FixedUpdate()
    {
        if (_controlsLocked || _isMovementLocked) return;

        // Apply Axis Locking
        Vector2 processedInput = _currentInput;
        if (lockAxisX) processedInput.x = 0;
        if (lockAxisY) processedInput.y = 0;

        HandleMovement(processedInput);
    }

    // --- Abstract Methods ---
    // Children MUST implement these
    protected abstract void HandleMovement(Vector2 input);
    protected abstract void HandleJump();
    
    // Helper to allow external scripts to lock specific axes
    public void SetAxisLock(bool x, bool y)
    {
        lockAxisX = x;
        lockAxisY = y;
    }
}
