using UnityEngine;

public class FeatherPickup : MonoBehaviour
{
    [Header("Feather Settings")]
    [SerializeField] private string featherName = "Mysterious Feather";
    [SerializeField] private bool isFirstFeather = false; // Mark the first feather in inspector
    
    private bool hasBeenInteracted = false;
    
    void Start()
    {
        // Check if feathers were refused
        if (GameStateManager.Instance != null && GameStateManager.Instance.refusedFeathers)
        {
            gameObject.SetActive(false);
        }
    }
    
    void OnInteract()
    {
        if (hasBeenInteracted) return;
        if (GameStateManager.Instance == null) return;
        
        hasBeenInteracted = true;
        
        // Show dialogue with choice
        string[] dialogue = new string[] {
            "You found a " + featherName + ". Will you pick it up?"
        };
        
        DialogueManager.Instance.StartDialogue(dialogue, "", () => {
            // After dialogue, show choices
            ShowPickupChoice();
        });
    }
    
    void ShowPickupChoice()
    {
        string[] choices = new string[] {
            "Pick it up",
            "Leave it"
        };
        
        DialogueManager.Instance.ShowChoices(choices, OnChoiceMade);
    }
    
    void OnChoiceMade(int choiceIndex)
    {
        if (choiceIndex == 0) // Pick it up
        {
            CollectFeather();
        }
        else // Leave it
        {
            RefuseFeather();
        }
    }
    
    void CollectFeather()
    {
        GameStateManager.Instance.CollectFeather();
        
        string[] successDialogue = new string[] {
            $"You collected the feather. ({GameStateManager.Instance.feathersCollected}/3)"
        };
        
        DialogueManager.Instance.StartDialogue(successDialogue, "", () => {
            gameObject.SetActive(false);
        });
    }
    
    void RefuseFeather()
    {
        if (isFirstFeather)
        {
            GameStateManager.Instance.RefuseFeather();
            
            string[] refuseDialogue = new string[] {
                "You decided to leave the feathers alone.",
                "All the feathers vanish mysteriously..."
            };
            
            DialogueManager.Instance.StartDialogue(refuseDialogue, "", () => {
                gameObject.SetActive(false);
            });
        }
        else
        {
            // If not first feather, just don't collect it
            string[] leaveDialogue = new string[] {
                "You left the feather where it was."
            };
            
            DialogueManager.Instance.StartDialogue(leaveDialogue, "", () => {
                gameObject.tag = "Untagged"; // Remove from interactables
            });
        }
    }
}