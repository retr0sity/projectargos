using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Place at the market exit. Player presses interact to return to saved position.
/// Tag the GameObject "Interactable" — InteractionDetector handles the rest.
/// </summary>
public class ReturnPortal : MonoBehaviour
{
    void Start()
    {
        if (gameObject.tag != "Interactable")
            gameObject.tag = "Interactable";
    }

    public void OnInteract()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager not found!");
            return;
        }

        if (string.IsNullOrEmpty(GameStateManager.Instance.returnSceneName))
        {
            Debug.LogError("No return scene saved — did you enter through a ScenePortal?");
            return;
        }

        GameStateManager.Instance.ReturnToSavedPosition();
    }
}