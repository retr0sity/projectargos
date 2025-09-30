using UnityEngine;

public class FogToggleTrigger : MonoBehaviour
{
    [Header("Fog Objects to Toggle")]
    [Tooltip("Drag the sprite follower GameObject here")]
    public GameObject fogFollowerSprite;
    
    [Tooltip("Drag the other fog controller GameObject here")]
    public GameObject fogController;
    
    [Header("Settings")]
    [Tooltip("What tag should trigger this? Usually 'Player'")]
    public string playerTag = "Player";
    
    [Tooltip("Cooldown time to prevent rapid toggling")]
    public float toggleCooldown = 0.5f;
    
    private float lastToggleTime = -1f;
    
    private void Start()
    {
        // Make sure we have a GameStateManager
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found! Make sure it exists in the scene.");
            return;
        }
        
        // Set fog to the saved state from GameStateManager
        SetFogState(GameStateManager.Instance.fogActive);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name + " with tag: " + other.tag);
        
        // Check if the object entering has the correct tag
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Correct tag detected! Current fog state: " + GameStateManager.Instance.fogActive);
            
            // Check cooldown to prevent rapid toggling
            if (Time.time < lastToggleTime + toggleCooldown)
            {
                Debug.Log("Toggle blocked by cooldown. Time remaining: " + (lastToggleTime + toggleCooldown - Time.time));
                return;
            }
            
            // Check if GameStateManager exists
            if (GameStateManager.Instance == null)
            {
                Debug.LogError("GameStateManager.Instance is null!");
                return;
            }
            
            // Update last toggle time
            lastToggleTime = Time.time;
            
            // Show before state
            Debug.Log("BEFORE toggle - GameStateManager.fogActive: " + GameStateManager.Instance.fogActive);
            
            // Toggle the fog state in GameStateManager
            GameStateManager.Instance.fogActive = !GameStateManager.Instance.fogActive;
            
            // Show after state
            Debug.Log("AFTER toggle - GameStateManager.fogActive: " + GameStateManager.Instance.fogActive);
            
            SetFogState(GameStateManager.Instance.fogActive);
            
            // Final confirmation
            Debug.Log("Fog toggle COMPLETE → " + (GameStateManager.Instance.fogActive ? "ON" : "OFF"));
        }
        else
        {
            Debug.Log("Wrong tag! Expected: " + playerTag + ", Got: " + other.tag);
        }
    }
    
    private void SetFogState(bool isActive)
    {
        // Toggle the fog follower sprite
        if (fogFollowerSprite != null)
        {
            fogFollowerSprite.SetActive(isActive);
        }
        
        // Toggle the fog controller
        if (fogController != null)
        {
            fogController.SetActive(isActive);
        }
    }
    
    // Optional: Show trigger area in scene view
    private void OnDrawGizmosSelected()
    {
        bool currentFogState = GameStateManager.Instance != null ? GameStateManager.Instance.fogActive : true;
        Gizmos.color = currentFogState ? Color.green : Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        
        // Draw trigger area (assumes you're using a BoxCollider2D or CircleCollider2D)
        BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
        if (boxCol != null)
        {
            Gizmos.DrawWireCube(Vector3.zero, boxCol.size);
        }
        
        CircleCollider2D circleCol = GetComponent<CircleCollider2D>();
        if (circleCol != null)
        {
            Gizmos.DrawWireSphere(Vector3.zero, circleCol.radius);
        }
    }
}
