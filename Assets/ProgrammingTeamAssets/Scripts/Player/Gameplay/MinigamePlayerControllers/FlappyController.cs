using UnityEngine;

public class FlappyController : BasePlayerController
{
    [Header("Flight Stats")]
    [SerializeField] private float autoScrollSpeed = 3f;
    [SerializeField] private float flapStrength = 5f;
    [SerializeField] private float maxFallSpeed = 8f;

    protected override void HandleMovement(Vector2 input)
    {
        // Force auto-scroll X unless locked
        float xVel = lockAxisX ? 0 : autoScrollSpeed;

        // Clamp falling speed so we don't fall too fast
        float yVel = Mathf.Clamp(_rb.linearVelocity.y, -maxFallSpeed, maxFallSpeed);

        _rb.linearVelocity = new Vector2(xVel, yVel);
    }

    protected override void HandleJump()
    {
        // Flap!
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0); // Reset Y momentum
        _rb.AddForce(Vector2.up * flapStrength, ForceMode2D.Impulse);
        
        _animator.SetTrigger("Flap"); // Make sure you have a Flap trigger
    }
}