using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformerController : BasePlayerController
{
    [Header("Platformer Stats")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private bool canRun = true;

    [Header("Checks")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;

    private bool _isGrounded;
    private bool _facingRight = false; // Assumes sprite faces right by default. CAREFULLY CHANGE THIS IF YOU CHANGE THE SPRITE!
    
    protected override void HandleMovement(Vector2 input)
    {
        // Determine Speed
        bool isSprinting = canRun && _isRunningInput;
        float targetSpeed = isSprinting ? runSpeed : walkSpeed;
        
        // Apply Velocity (X only, keep Y gravity)
        _rb.linearVelocity = new Vector2(input.x * targetSpeed, _rb.linearVelocity.y);

        // Animation
        // Normalize speed for blend tree so animations look correct regardless of speed
        float animSpeed = Mathf.Abs(input.x) * (targetSpeed / runSpeed);
        _animator.SetFloat("Speed", animSpeed);

        // Flip Logic
        if (input.x > 0.01f && !_facingRight) Flip();
        else if (input.x < -0.01f && _facingRight) Flip();
    }

    protected override void HandleJump()
    {
        // Prevent jump in "death" scene (carried over from your old script)
        if (SceneManager.GetActiveScene().name == "death") return;

        if (_isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _animator.SetBool("IsJumping", true);
        }
    }

    private void Update()
    {
        // Ground Check
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Update Animator Land/Jump state
        if (_isGrounded)
        {
            _animator.SetBool("IsJumping", false);
        }
        else
        {
            // Just simple falling logic
            if (_rb.linearVelocity.y > 0.1f) _animator.SetBool("IsJumping", true);
            else if (_rb.linearVelocity.y < -0.1f) _animator.SetBool("IsJumping", false);
        }
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