using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Forward portal that transports player to another scene (like mini-game).
/// Saves player's current position so they can return later.
/// Requires "Interactable" tag and GameStateManager in scene.
/// Each portal tracks its own usage independently.
/// </summary>
public class ScenePortal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private string targetSceneName = "";
    [SerializeField] private bool oneTimeUse = true;
    [SerializeField] private string portalID = ""; // Unique ID for this portal
    
    [Header("Return Position")]
    [Tooltip("Assign a Transform (like an empty GameObject) to specify an exact return spot. If left empty, the portal's own position will be used as the return point.")]
    [SerializeField] private Transform customReturnPoint; // <-- NEW VARIABLE
    
    void Awake()
    {
        if (gameObject.tag == "Untagged")
            gameObject.tag = "Interactable";
        
        // Auto-generate portal ID if not set
        if (string.IsNullOrEmpty(portalID))
        {
            portalID = $"{SceneManager.GetActiveScene().name}_{gameObject.name}";
        }
    }
    
    void Start()
    {
        // Check if THIS specific portal was already used
        if (oneTimeUse && GameStateManager.Instance != null && GameStateManager.Instance.HasPortalBeenUsed(portalID))
        {
            gameObject.SetActive(false);
        }
    }
    
    public void OnInteract()
    {
        Debug.Log($"[Portal] OnInteract called, target: {targetSceneName}");
        if (oneTimeUse && GameStateManager.Instance.HasPortalBeenUsed(portalID)) 
            return;
        
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found in scene!");
            return;
        }
        
        // We still check for the player just to make sure they exist
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Cannot save return position.");
            return;
        }
        
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError($"No target scene set for portal {gameObject.name}");
            return;
        }
        
        // Mark THIS portal as used
        if (oneTimeUse)
            GameStateManager.Instance.MarkPortalAsUsed(portalID);
        
        // --- UPDATED LOGIC ---
        // Determine the return position
        Vector3 returnPos;
        if (customReturnPoint != null)
        {
            // Use the specific position from the Transform you assigned
            returnPos = customReturnPoint.position;
        }
        else
        {
            // Default to the portal's own position
            returnPos = transform.position; 
        }
        
        // Save the chosen return point
        GameStateManager.Instance.SetReturnPoint(
            SceneManager.GetActiveScene().name,
            returnPos
        );
        // --- END OF UPDATED LOGIC ---
        
        // Load target scene
        SceneManager.LoadScene(targetSceneName);
    }
    
    public void ResetPortal()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.ResetPortal(portalID);
        
        gameObject.SetActive(true);
    }
}