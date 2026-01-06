using UnityEngine;

public class TopDownController : BasePlayerController
{
    [Header("TopDown Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool spriteFacesRight = true; // Uncheck this if your art faces Left by default

    protected override void OnEnable()
    {
        base.OnEnable();
        // Disable gravity so the player doesn't fall down the screen
        if (_rb != null) _rb.gravityScale = 0f; 
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        // Restore gravity in case we switch back to a Platformer section
        if (_rb != null) _rb.gravityScale = 1f; 
    }

    protected override void HandleMovement(Vector2 input)
    {
        // 1. Apply Movement (X and Y)
        _rb.linearVelocity = input * moveSpeed;

        // 2. Animate (Uses Magnitude 0 to 1)
        if (_animator != null)
        {
            _animator.SetFloat("Speed", input.magnitude);
        }
        
        // 3. Robust Facing Logic (Fixes Moon Walking)
        if (Mathf.Abs(input.x) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            
            // Determine desired direction based on Input
            float direction = input.x > 0 ? 1 : -1;

            // If the art itself faces Left, we need to invert the math
            if (!spriteFacesRight) direction *= -1;

            // Apply the new scale (using Abs ensures we don't flip-flop incorrectly)
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }
    }

    protected override void HandleJump()
    {
        // Top-Down games usually don't have a "Jump".
        // You can leave this empty, or add a Dash/Interact logic here.
    }
}