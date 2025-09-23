using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Forward portal that transports player to another scene (like mini-game).
/// Saves player's current position so they can return later.
/// Requires "Interactable" tag and GameStateManager in scene.
/// </summary>
public class ScenePortal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private string targetSceneName = "MiniGame";
    [SerializeField] private bool oneTimeUse = true;
    
    void Awake()
    {
        if (gameObject.tag == "Untagged")
            gameObject.tag = "Interactable";
    }
    
    void Start()
    {
        // Check if portal was already used
        if (oneTimeUse && GameStateManager.Instance != null && GameStateManager.Instance.hasUsedPortal)
        {
            gameObject.SetActive(false);
        }
    }
    
    public void OnInteract()
    {
        if (oneTimeUse && GameStateManager.Instance.hasUsedPortal) 
            return;
        
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found in scene!");
            return;
        }
        
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
        
        // Mark portal as used in persistent state
        if (oneTimeUse)
            GameStateManager.Instance.hasUsedPortal = true;
        
        // Save return point
        GameStateManager.Instance.SetReturnPoint(
            SceneManager.GetActiveScene().name,
            player.transform.position
        );
        
        // Load mini-game
        SceneManager.LoadScene(targetSceneName);
    }
    
    public void ResetPortal()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.hasUsedPortal = false;
        gameObject.SetActive(true);
    }
}