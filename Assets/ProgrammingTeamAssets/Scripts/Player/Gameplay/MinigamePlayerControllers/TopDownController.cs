using UnityEngine;

public class TopDownController : BasePlayerController
{
    [Header("TopDown Stats")]
    [SerializeField] private float moveSpeed = 5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (_rb != null) _rb.gravityScale = 0f; // Disable gravity
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_rb != null) _rb.gravityScale = 1f; // Re-enable gravity for other modes
    }

    protected override void HandleMovement(Vector2 input)
    {
        // Move in X and Y
        _rb.linearVelocity = input * moveSpeed;

        // Animation
        _animator.SetFloat("Speed", input.magnitude);
        
        // Optional: Add 4-direction facing logic here if you have 4-way sprites
        if (input.x > 0.01f) transform.localScale = new Vector3(1,1,1);
        else if (input.x < -0.01f) transform.localScale = new Vector3(-1,1,1);
    }

    protected override void HandleJump()
    {
        // Top down usually doesn't jump, maybe Dash?
        Debug.Log("Jump/Dash Action");
    }
}