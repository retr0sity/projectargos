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
    }

    private void OnDisable()
    {
        var input = InputManager.Instance;
        if (input != null)
        {
            input.MoveEvent -= HandleMove;
            input.JumpEvent -= HandleJump;
        }
    }

    private void HandleMove(Vector2 movement)
    {
        // Store horizontal input
        _moveInput = new Vector2(movement.x * runSpeed, _rb.linearVelocity.y);
        _animator.SetFloat("Speed", Mathf.Abs(movement.x));

        // Flip character if needed
        if (movement.x > 0 && !_facingRight) Flip();
        else if (movement.x < 0 && _facingRight) Flip();
    }

    private void HandleJump()
    {
        if (!_isGrounded || SceneManager.GetActiveScene().name == "death") return;

        _jumpRequested = true;
    }

    private void FixedUpdate()
    {
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
