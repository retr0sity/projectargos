using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private bool isReturnPortal = false;
    [SerializeField] private bool oneTimeUse = true;
    
    private bool hasBeenUsed = false;
    
    void OnInteract()
    {
        if (oneTimeUse && hasBeenUsed) return;
        
        hasBeenUsed = true;
        
        if (isReturnPortal)
        {
            // Return to saved position
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.ReturnToSavedPosition();
            }
        }
        else
        {
            // Save current position and go to new scene
            if (GameStateManager.Instance != null)
            {
                // Save return point near the portal
                GameStateManager.Instance.SetReturnPoint(
                    SceneManager.GetActiveScene().name,
                    transform.position
                );
            }
            
            // Load target scene
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }
    }
}