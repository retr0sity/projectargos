using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class RigPlayerController : BasePlayerController
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private bool canRun = false; // Locked by default

    private bool _facingRight = false;
    private bool _isGrounded;
    private bool _jumpRequested;
    private float _inputX;

    protected override void OnEnable()
    {
        base.OnEnable();
        _jumpRequested = false;
        _inputX = 0f;
    }

    protected override void OnDisable()
    {
        _jumpRequested = false;
        base.OnDisable();
    }

    private void Update()
    {
        if (groundCheck == null || _animator == null || _rb == null) return;

        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (_isGrounded)
        {
            _animator.SetBool("IsJumping", false);
        }
        else if (_rb.linearVelocity.y > 0.1f)
        {
            _animator.SetBool("IsJumping", true);
        }
        else if (_rb.linearVelocity.y < -0.1f)
        {
            _animator.SetBool("IsJumping", false);
        }
    }

    public void UnlockRun()
    {
        canRun = true;
    }

    public void LockRun()
    {
        canRun = false;
        _isRunningInput = false;
    }

    protected override void HandleMovement(Vector2 input)
    {
        if (_rb == null || _animator == null) return;

        _inputX = input.x;
        bool isRunning = canRun && _isRunningInput;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        _rb.linearVelocity = new Vector2(_inputX * currentSpeed, _rb.linearVelocity.y);

        float normalizedRunSpeed = runSpeed > 0.01f ? currentSpeed / runSpeed : 1f;
        _animator.SetFloat("Speed", Mathf.Abs(_inputX) * normalizedRunSpeed);

        if (_inputX > 0.01f && !_facingRight) Flip();
        else if (_inputX < -0.01f && _facingRight) Flip();

        if (_jumpRequested)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _jumpRequested = false;
        }
    }

    protected override void HandleJump()
    {
        if (!_isGrounded || SceneManager.GetActiveScene().name == "death") return;
        _jumpRequested = true;
    }

    protected override void StopMovement()
    {
        base.StopMovement();
        _jumpRequested = false;
        _inputX = 0f;
    }

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
