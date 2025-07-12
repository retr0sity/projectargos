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

    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _moveInput;
    private bool _jumpRequested;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
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

        // Subscribe to input events
        input.MoveEvent += HandleMove;
        input.JumpEvent += HandleJump;
        input.LandingEvent += HandleLanding;
    }

    private void OnDisable()
    {
        var input = InputManager.Instance;
        if (input != null)
        {
            input.MoveEvent -= HandleMove;
            input.JumpEvent -= HandleJump;
            input.LandingEvent -= HandleLanding;
        }
    }

    private void HandleMove(Vector2 movement)
    {
        // Store horizontal input; vertical velocity handled by physics
        _moveInput = new Vector2(movement.x * runSpeed, _rb.linearVelocity.y);
        _animator.SetFloat("Speed", Mathf.Abs(movement.x));
    }

    private void HandleJump()
    {
        if (SceneManager.GetActiveScene().name == "death") return;
        _jumpRequested = true;
        _animator.SetBool("IsJumping", true);
    }

    private void FixedUpdate()
    {
        // Preserve current velocity for gravity
        Vector2 velocity = _rb.linearVelocity;

        // Apply horizontal movement
        velocity.x = _moveInput.x;

        // Apply jump impulse
        if (_jumpRequested)
        {
            velocity.y = jumpForce;
            _jumpRequested = false;
        }

        _rb.linearVelocity = velocity;
    }

    private void HandleLanding()
    {
        _animator.SetBool("IsJumping", false);
    }
}
