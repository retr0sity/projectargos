using System;
using UnityEngine;
using Core.Managers;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class RigPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private bool canRun = false; // Locked by default

    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _moveInput;
    private bool _jumpRequested;
    private bool _facingRight = false;
    private bool _isGrounded;
    private bool _controlsLocked = false;
    private bool _isWalking;
    private float _inputX; // store raw horizontal input

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Always check if grounded
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (_isGrounded)
        {
            _animator.SetBool("IsJumping", false);
        }
        else
        {
            if (_rb.linearVelocity.y > 0.1f)
            {
                _animator.SetBool("IsJumping", true);
            }
            else if (_rb.linearVelocity.y < -0.1f)
            {
                _animator.SetBool("IsJumping", false);
            }
        }
        
        // Walk by default, run if Shift is held
        _isWalking = !Input.GetKey(KeyCode.LeftShift) || !canRun;

        // Update animator speed even if input hasn't changed
        float currentSpeed = _isWalking ? walkSpeed : runSpeed;
        _animator.SetFloat("Speed", Mathf.Abs(_inputX) * (currentSpeed / runSpeed));
    }

    private void OnEnable()
    {
        var input = InputManager.Instance;
        if (input == null)
        {
            Debug.LogError("PlayerController could not find InputManager in the scene. Please ensure an InputManager is present before this component.");
            enabled = false;
            return;
        }

        input.MoveEvent += HandleMove;
        input.JumpEvent += HandleJump;
        input.ControlLockChanged += OnControlLockChanged;
        
        // FIX: Reset ALL movement state when enabled
        _moveInput = Vector2.zero;
        _inputX = 0f; // ← CRITICAL FIX: Clear input state
        _controlsLocked = false;
    }

    private void OnDisable()
    {
        var input = InputManager.Instance;
        if (input != null)
        {
            input.MoveEvent -= HandleMove;
            input.JumpEvent -= HandleJump;
            input.ControlLockChanged -= OnControlLockChanged;
        }
        
        // Stop all movement when disabled
        StopMovement();
    }
    
    private void HandleWalk(bool isWalking)
    {
        _isWalking = isWalking;
    }

    public void UnlockRun()
    {
        canRun = true;
    }

    public void LockRun()
    {
        canRun = false;
    }


    
    private void OnControlLockChanged(bool isLocked)
    {
        _controlsLocked = isLocked;

        // When controls are locked, stop movement immediately
        if (isLocked)
        {
            StopMovement();
        }
    }
    
    private void StopMovement()
    {
        _moveInput = Vector2.zero;
        _inputX = 0f; // FIX: Clear input state
        
        if (_rb != null)
        {
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
        }
        if (_animator != null)
        {
            _animator.SetFloat("Speed", 0f);
        }
    }

    private void HandleMove(Vector2 movement)
    {
        if (_controlsLocked) return;

        _inputX = movement.x;

        // Flip sprite if input direction changes
        if (_inputX > 0.01f && !_facingRight) Flip();
        else if (_inputX < -0.01f && _facingRight) Flip();

        // Set animator speed relative to movement speed
        float currentSpeed = _isWalking ? walkSpeed : runSpeed;
        _animator.SetFloat("Speed", Mathf.Abs(_inputX) * (currentSpeed / runSpeed));
    }

    private void HandleJump()
    {
        // Don't process jump if controls are locked
        if (_controlsLocked) return;
        
        if (!_isGrounded || SceneManager.GetActiveScene().name == "death") return;

        _jumpRequested = true;
    }

    private void FixedUpdate()
    {
        if (_controlsLocked)
        {
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            return;
        }

        // Walk by default, run if Shift is pressed
        float currentSpeed = _isWalking ? walkSpeed : runSpeed;

        Vector2 velocity = new Vector2(_inputX * currentSpeed, _rb.linearVelocity.y);

        if (_jumpRequested)
        {
            velocity.y = jumpForce;
            _jumpRequested = false;
        }

        _rb.linearVelocity = velocity;
    }

    /// <summary>
    /// Flips the player's sprite horizontally
    /// </summary>
    private void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}