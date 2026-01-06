using UnityEngine;

public class FlappyController : BasePlayerController
{
    [Header("Flappy Physics")]
    [SerializeField] private float autoScrollSpeed = 3f;
    [SerializeField] private float flapStrength = 8f;
    [SerializeField] private float gravityScale = 2.5f;
    [SerializeField] private float maxFallSpeed = 10f;

    [Header("Visuals")]
    [SerializeField] private bool rotateSprite = true;
    [SerializeField] private float rotationSpeed = 5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_rb != null) 
        {
            // Flappy bird needs strong gravity to feel "heavy" and responsive
            _rb.gravityScale = gravityScale; 
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        // Reset rotation when leaving this mode
        transform.rotation = Quaternion.identity;
        // Reset gravity to normal
        if (_rb != null) _rb.gravityScale = 1f; 
    }

    protected override void HandleMovement(Vector2 input)
    {
        // 1. Calculate Velocity
        // Auto-scroll X (Constant speed), unless Axis X is locked by the game
        float xVel = lockAxisX ? 0 : autoScrollSpeed;

        // Clamp falling speed (Terminal velocity)
        float yVel = Mathf.Clamp(_rb.linearVelocity.y, -maxFallSpeed, 100f);

        _rb.linearVelocity = new Vector2(xVel, yVel);

        // 2. Rotation Logic (Nose Dive)
        if (rotateSprite)
        {
            // Calculate angle based on vertical speed
            // Flying up = Positive Angle, Falling down = Negative Angle
            float targetAngle = Mathf.Atan2(_rb.linearVelocity.y, 10) * Mathf.Rad2Deg;
            
            // Smoothly rotate towards that angle
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    protected override void HandleJump()
    {
        if (_rb == null) return;

        // CRITICAL: Reset vertical velocity to 0 before adding force.
        // This ensures the jump height is CONSISTENT regardless of how fast you were falling.
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
        
        // Apply immediate upward force
        _rb.AddForce(Vector2.up * flapStrength, ForceMode2D.Impulse);
        
        // Animation
        if (_animator != null) _animator.SetTrigger("Flap");
    }
}