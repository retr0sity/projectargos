using UnityEngine;

/// <summary>
/// Automatic trigger that returns player to saved position when entered.
/// Place this in the final scene of your mini-game. Player walks into it = instant return.
/// Requires GameStateManager in scene.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AutoReturnTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool oneTimeUse = true;
    
    private bool hasBeenUsed = false;
    
    void Awake()
    {
        // Ensure this is a trigger collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) 
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogError($"{gameObject.name} needs a Collider2D component to work as a trigger!");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger for player
        if (!other.CompareTag("Player")) return;
        
        // Check if already used
        if (oneTimeUse && hasBeenUsed) return;
        
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found! Cannot return player.");
            return;
        }
        
        hasBeenUsed = true;
        
        // Automatically return player to saved position in main scene
        GameStateManager.Instance.ReturnToSavedPosition();
    
    }
    
    public void CutsceneReturn()
    {
        GameStateManager.Instance.ReturnToSavedPosition();
    }

    /// <summary>
    /// Reset trigger for testing
    /// </summary>
    public void ResetTrigger()
    {
        hasBeenUsed = false;
    }
}