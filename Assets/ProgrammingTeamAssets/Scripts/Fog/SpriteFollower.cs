using UnityEngine;

public class SpriteFollower : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Drag the player GameObject here")]
    public Transform player;
    
    [Header("Position Settings")]
    [Tooltip("Offset when player moves RIGHT")]
    public Vector2 rightMovementOffset = Vector2.zero;
    
    [Tooltip("Offset when player moves LEFT")]
    public Vector2 leftMovementOffset = Vector2.zero;
    
    [Tooltip("Base center offset (applied to both left and right)")]
    public Vector2 baseCenterOffset = Vector2.zero;
    
    [Header("Movement Settings")]
    [Tooltip("How fast the sprite follows the player")]
    public float followSpeed = 5f;
    
    [Tooltip("How fast the sprite transitions between left/right positions")]
    public float transitionSpeed = 8f;
    
    [Tooltip("How sensitive direction detection is (lower = more sensitive)")]
    public float directionThreshold = 0.1f;
    
    [Header("Debug")]
    [Tooltip("Show debug information")]
    public bool showDebug = false;
    
    [Tooltip("Show gizmos for left and right positions")]
    public bool showPositionGizmos = true;
    
    private Vector3 lastPlayerPosition;
    private float currentMovementDirection = 0f; // -1 = left, 0 = stationary, 1 = right
    private Vector2 currentDirectionalOffset = Vector2.zero;
    
    private void Start()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
    }
    
    private void Update()
    {
        // Check if player reference exists
        if (player == null)
        {
            Debug.LogWarning("Player reference not set on " + gameObject.name);
            return;
        }
        
        // Update movement direction smoothly
        UpdateMovementDirection();
        
        // Smoothly transition between left/right offsets
        UpdateDirectionalOffset();
        
        // Calculate final target position
        Vector3 targetPosition = CalculateTargetPosition();
        
        // Move towards target smoothly
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        
        // Debug info
        if (showDebug)
        {
            Debug.Log($"Direction: {currentMovementDirection:F2}, Current Offset: {currentDirectionalOffset}, Target: {targetPosition}");
        }
    }
    
    private void UpdateMovementDirection()
    {
        float deltaX = player.position.x - lastPlayerPosition.x;
        
        // Smooth direction detection
        if (Mathf.Abs(deltaX) > directionThreshold * Time.deltaTime)
        {
            if (deltaX > 0)
            {
                currentMovementDirection = Mathf.Lerp(currentMovementDirection, 1f, Time.deltaTime * 5f); // Moving right
            }
            else
            {
                currentMovementDirection = Mathf.Lerp(currentMovementDirection, -1f, Time.deltaTime * 5f); // Moving left
            }
        }
        else
        {
            // Player is stationary, slowly return to neutral
            currentMovementDirection = Mathf.Lerp(currentMovementDirection, 0f, Time.deltaTime * 2f);
        }
        
        lastPlayerPosition = player.position;
    }
    
    private void UpdateDirectionalOffset()
    {
        Vector2 targetOffset;
        
        // Determine target offset based on movement direction
        if (currentMovementDirection > 0.1f) // Moving right
        {
            targetOffset = rightMovementOffset;
        }
        else if (currentMovementDirection < -0.1f) // Moving left
        {
            targetOffset = leftMovementOffset;
        }
        else // Stationary or neutral
        {
            // Blend between left and right based on last direction
            float blendFactor = (currentMovementDirection + 1f) * 0.5f; // Convert -1,1 to 0,1
            targetOffset = Vector2.Lerp(leftMovementOffset, rightMovementOffset, blendFactor);
        }
        
        // Smoothly transition to target offset
        currentDirectionalOffset = Vector2.Lerp(currentDirectionalOffset, targetOffset, transitionSpeed * Time.deltaTime);
    }
    
    private Vector3 CalculateTargetPosition()
    {
        Vector2 totalOffset = baseCenterOffset + currentDirectionalOffset;
        
        return new Vector3(
            player.position.x + totalOffset.x,
            player.position.y + totalOffset.y,
            transform.position.z  // Keep the same Z position
        );
    }
    
    // Draw gizmos for easy position adjustment
    private void OnDrawGizmosSelected()
    {
        if (player == null || !showPositionGizmos) return;
        
        Vector3 basePos = new Vector3(
            player.position.x + baseCenterOffset.x,
            player.position.y + baseCenterOffset.y,
            transform.position.z
        );
        
        // Draw base center position
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(basePos, 0.15f);
        
        // Draw left position
        Vector3 leftPos = basePos + (Vector3)leftMovementOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftPos, 0.2f);
        Gizmos.DrawLine(basePos, leftPos);
        
        // Draw right position
        Vector3 rightPos = basePos + (Vector3)rightMovementOffset;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(rightPos, 0.2f);
        Gizmos.DrawLine(basePos, rightPos);
        
        // Draw current target
        Vector3 currentTarget = CalculateTargetPosition();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(currentTarget, 0.1f);
        Gizmos.DrawLine(transform.position, currentTarget);
        
        // Draw labels in scene view
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(leftPos + Vector3.up * 0.3f, "LEFT");
        UnityEditor.Handles.Label(rightPos + Vector3.up * 0.3f, "RIGHT");
        UnityEditor.Handles.Label(basePos + Vector3.up * 0.5f, "BASE");
        #endif
    }
}
