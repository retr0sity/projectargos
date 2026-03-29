using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Place at the market exit. Player presses interact to return to saved position.
/// Tag the GameObject "Interactable" — InteractionDetector handles the rest.
/// Assign Full_Cart_Monologue to warn the player once when they try to leave
/// with items still in the cart; the second exit attempt always goes through.
/// </summary>
public class ReturnPortal : MonoBehaviour
{
    [Header("Cart Warning")]
    [Tooltip("Assign the Full_Cart_Monologue GameObject's SimpleNPCDialogue here.")]
    [SerializeField] private SimpleNPCDialogue fullCartMonologue;

    private bool hasWarned = false;

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

        bool cartHasItems = CartManager.Instance != null && CartManager.Instance.items.Count > 0;

        if (cartHasItems && fullCartMonologue != null && !hasWarned)
        {
            hasWarned = true;
            fullCartMonologue.gameObject.SetActive(true);
            fullCartMonologue.TriggerNow();
            return;
        }

        GameStateManager.Instance.ReturnToSavedPosition();
    }
}