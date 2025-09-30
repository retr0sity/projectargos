using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Final interaction that shows different endings based on feather collection.
/// Requires "Interactable" tag, DialogueManager, and GameStateManager in scene.
/// FIXED: Better state management to prevent double-interaction
/// </summary>
public class EndingNPCDialogue : MonoBehaviour
{
    [Header("Dialogue - No Feathers")]
    [TextArea(3,5)]
    [SerializeField] private string[] automaticEndingDialogue = {
        "One day you will learn how to use them."
    };
    
    [Header("Dialogue - Endings")]
    [TextArea(3,5)]
    [SerializeField] private string[] ending1Dialogue = {
        "You're kind.",
        "But you need them more than I do."
    };
    
    [TextArea(3,5)]
    [SerializeField] private string[] ending3Dialogue = {
        "You will get them back when you know how to use them."
    };
    
    [Header("Sky Text")]
    [SerializeField] private GameObject skyTextObject;
    [SerializeField] private string skyMessage = "The sea will bring them back when you need them.";

    [Header("Sand Burial Camera")]
    [SerializeField] private DelayedCameraEvent sandBurialCamera;
    
    [Header("Settings")]
    [SerializeField] private float delayBeforeCredits = 3f;
    
    private bool hasBeenInteracted = false;
    private bool isProcessingEnding = false; // FIX: Prevent double-interaction
    
    /// <summary>
    /// Called by InteractionDetector when player presses interact
    /// FIX: Added check to prevent double-interaction
    /// </summary>
    public void OnInteract()
    {
        // FIX: Prevent double-interaction
        if (hasBeenInteracted || isProcessingEnding) return;
        
        if (GameStateManager.Instance == null || DialogueManager.Instance == null)
        {
            Debug.LogError("Missing required managers!");
            return;
        }
        
        // Check if visited mini-game scene first
        if (!GameStateManager.Instance.hasVisitedOtherScene)
        {
            DialogueManager.Instance.ShowMonologue("You should explore the console first.");
            return;
        }
        
        hasBeenInteracted = true;
        isProcessingEnding = true;
        
        // Check feather status
        if (GameStateManager.Instance.refusedFeathers || 
            GameStateManager.Instance.feathersCollected == 0)
        {
            // No feathers collected - automatic ending
            ShowNoFeatherEnding();
        }
        else
        {
            // Has at least 1 feather - show choices
            ShowEndingChoices();
        }
    }
    
    void ShowNoFeatherEnding()
    {
        GameStateManager.Instance.endingChosen = 3;
        
        DialogueManager.Instance.StartDialogue(
            automaticEndingDialogue, 
            "Stork",
            () => {
                Invoke("LoadCredits", delayBeforeCredits);
            }
        );
    }
    
    void ShowEndingChoices()
    {
        string[] choices = {
            "Make a bouquet and give them",
            "Bury them in the sand", 
            "Pierce your ears with them"
        };
        
        DialogueManager.Instance.ShowChoices(choices, OnEndingChoice);
    }
    
    void OnEndingChoice(int choice)
    {
        GameStateManager.Instance.endingChosen = choice;
        
        switch (choice)
        {
            case 0: // Bouquet (ending 0)
                DialogueManager.Instance.StartDialogue(
                    ending1Dialogue, 
                    "Stork",
                    () => Invoke("LoadCredits", delayBeforeCredits)
                );
                break;
                
            case 1: // Sand (ending 1)
                // Trigger camera zoom/pan before showing text
                if (sandBurialCamera != null)
                {
                    sandBurialCamera.TriggerSequence();
                }
                else
                {
                    // Fallback to sky text
                    if (skyTextObject != null)
                    {
                        var skyText = skyTextObject.GetComponent<SkyText>();
                        if (skyText) skyText.ShowText(skyMessage);
                    }
                }
                
                Invoke("LoadCredits", delayBeforeCredits);
                break;
                
            case 2: // Pierce ears (ending 2)
                DialogueManager.Instance.StartDialogue(
                    ending3Dialogue, 
                    "Stork",
                    () => Invoke("LoadCredits", delayBeforeCredits)
                );
                break;
        }
    }
    
    void LoadCredits()
    {
        GameStateManager.Instance.LoadCredits();
    }
}