using System;
using UnityEngine;
using Core.Managers;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;

    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _moveInput;
    private bool _jumpRequested;
    private bool _facingRight = true;
    private bool _isGrounded;
    private bool _controlsLocked = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Ground checking each frame
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        _animator.SetBool("IsJumping", !_isGrounded);
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
        
        // Reset movement when enabled
        _moveInput = Vector2.zero;
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
        // Don't process movement if controls are locked
        if (_controlsLocked) return;
        
        // Store horizontal input
        _moveInput = new Vector2(movement.x * runSpeed, _rb.linearVelocity.y);
        _animator.SetFloat("Speed", Mathf.Abs(movement.x));

        // Flip character if needed
        if (movement.x > 0 && !_facingRight) Flip();
        else if (movement.x < 0 && _facingRight) Flip();
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
        // Don't apply any movement if controls are locked
        if (_controlsLocked)
        {
            // Make sure we're stopped
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            return;
        }
        
        // Apply horizontal movement
        Vector2 velocity = new Vector2(_moveInput.x, _rb.linearVelocity.y);

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