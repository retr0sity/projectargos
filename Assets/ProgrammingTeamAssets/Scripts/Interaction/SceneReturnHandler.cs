using UnityEngine;

public class SceneReturnHandler : MonoBehaviour
{
    void Start()
    {
        // Try to position player at return point if coming back
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.TryPositionPlayerAtReturn();
        }
        
        // Also refresh the interaction detector's cache of interactables
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            InteractionDetector detector = player.GetComponent<InteractionDetector>();
            if (detector != null)
            {
                detector.RefreshInteractables();
            }
        }
    }
}