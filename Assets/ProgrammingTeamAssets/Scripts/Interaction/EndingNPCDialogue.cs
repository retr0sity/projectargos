using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// Final interaction that shows different endings based on feather collection.
/// Requires "Interactable" tag, DialogueManager, and GameStateManager in scene.
/// </summary>
public class EndingNPCDialogue : MonoBehaviour
{
    [SerializeField] private Animator pelargosAnimator;
    [SerializeField] private string interactBool = "IsInteracting";
    [SerializeField] private string flightBool = "IsFlying";
    [SerializeField] private Flight flightScript;


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
    
    /// <summary>
    /// Called by InteractionDetector when player presses interact
    /// </summary>
    public void OnInteract()
    {
        if (hasBeenInteracted) return;
        
        if (GameStateManager.Instance == null || DialogueManager.Instance == null)
        {
            Debug.LogError("Missing required managers!");
            return;
        }

        // Play interaction animation
        if (pelargosAnimator != null)
        {
            pelargosAnimator.SetBool(interactBool, true);
        }

        
        // Check if visited mini-game scene first
        if (!GameStateManager.Instance.hasVisitedOtherScene)
        {
            DialogueManager.Instance.ShowMonologue("You should explore the console first.");
            StartCoroutine(ResetInteraction());
            return;
        }
        
        hasBeenInteracted = true;
        
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
    
    System.Collections.IEnumerator ResetInteraction()
    {
        yield return new WaitForSeconds(5f);
        if (pelargosAnimator != null)
        {
            pelargosAnimator.SetBool(interactBool, false);
        }
    }
    void ShowNoFeatherEnding()
    {
        GameStateManager.Instance.endingChosen = 3;

        DialogueManager.Instance.StartDialogue(
            automaticEndingDialogue,
            "Stork",
            () =>
            {
                Invoke("LoadCredits", delayBeforeCredits);
            }
        );

        // Trigger flight animation and movement
        if (pelargosAnimator != null)
        {
            pelargosAnimator.SetBool(flightBool, true);
        }
        if (flightScript != null)
        {
            flightScript.StartFlying();
        }
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
                    // NEW: Trigger camera zoom/pan before showing text
                if (sandBurialCamera != null)
                {
                    sandBurialCamera.TriggerSequence();
                }
                else
                {
                    // Fallback to old method
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
    
        // Trigger flight animation and movement
        if (pelargosAnimator != null)
        {
            pelargosAnimator.SetBool(flightBool, true);
        }
        if (flightScript != null)
        {
            flightScript.StartFlying();
        }
}
    
    void LoadCredits()
    {
        GameStateManager.Instance.LoadCredits();
    }
}