using UnityEngine;

/// <summary>
/// Collectible feather that player can choose to pick up or refuse.
/// First feather refusal destroys all other feathers.
/// </summary>
public class FeatherPickup : MonoBehaviour
{
    [Header("Feather Settings")]
    [SerializeField] private int featherNumber = 1; // For display purposes (1, 2, or 3)
    [SerializeField] private bool isFirstFeather = false;
    
    [Header("Sky Text")]
    [SerializeField] private GameObject skyTextObject;
    [SerializeField] private string skyMessage = "Take care of it.";
    
    private bool hasBeenInteracted = false;
    
    void Awake()
    {
        // Ensure interactable
        if (gameObject.tag == "Untagged")
            gameObject.tag = "Interactable";
    }
    
    void Start()
    {
        // Check if feathers were refused - if so, destroy this feather
        if (GameStateManager.Instance != null && GameStateManager.Instance.refusedFeathers)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Called by InteractionDetector when player presses interact
    /// </summary>
    public void OnInteract()
    {
        if (hasBeenInteracted) return;
        
        if (DialogueManager.Instance == null || GameStateManager.Instance == null)
        {
            Debug.LogError("Missing required managers!");
            return;
        }
        
        hasBeenInteracted = true;
        
        // Show sky text if configured
        if (skyTextObject != null)
        {
            SkyText skyText = skyTextObject.GetComponent<SkyText>();
            if (skyText != null) skyText.ShowText(skyMessage);
        }
        
        // Show choice directly (no dialogue step)
        ShowChoices();
    }
    
    void ShowChoices()
    {
        string[] choices = { 
            $"Pick up feather {featherNumber}", 
            "Leave it behind" 
        };
        
        DialogueManager.Instance.ShowChoices(choices, OnChoice);
    }
    
    void OnChoice(int choice)
    {
        if (choice == 0) // Pick up
        {
            GameStateManager.Instance.CollectFeather();
            
            string message = $"You collected feather {featherNumber}. ({GameStateManager.Instance.feathersCollected}/3)";
            DialogueManager.Instance.ShowMonologue(message);
            
            Destroy(gameObject);
        }
        else // Leave it
        {
            if (isFirstFeather)
            {
                // Refusing first feather destroys ALL feathers
                GameStateManager.Instance.RefuseFeather();
                DialogueManager.Instance.ShowMonologue("The feathers vanish into the wind...");
            }
            else
            {
                // Non-first feather: just make it disappear
                DialogueManager.Instance.ShowMonologue("You left the feather behind.");
                Destroy(gameObject);
            }
        }
    }
}