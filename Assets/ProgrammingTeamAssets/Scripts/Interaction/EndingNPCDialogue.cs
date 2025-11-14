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
    private bool isCurrentlyInDialogue = false; // prevent re-trigger

    /// <summary>
    /// Called by InteractionDetector when player presses interact
    /// </summary>
    public void OnInteract()
    {
        // ✅ safety checks
        if (GameStateManager.Instance == null || DialogueManager.Instance == null)
        {
            Debug.LogError("Missing required managers!");
            return;
        }

        // ✅ block new interactions while any dialogue or UI is active
        if (DialogueManager.Instance.IsAnyUIActive())
        {
            Debug.Log("Dialogue or other UI active — ignoring new interaction.");
            return;
        }

        // ✅ block repeat presses mid-dialogue
        if (isCurrentlyInDialogue)
        {
            Debug.Log("Already in dialogue, ignoring interaction.");
            return;
        }

        // ✅ optional: prevent repeat after one complete run
        if (hasBeenInteracted)
        {
            Debug.Log("Already interacted — ignoring repeat press.");
            return;
        }

        // ✅ play animation
        if (pelargosAnimator != null)
            pelargosAnimator.SetBool(interactBool, true);

        if (!GameStateManager.Instance.hasVisitedOtherScene)
        {
            isCurrentlyInDialogue = true; // lock until dialogue ends

            DialogueManager.Instance.StartDialogue(
                new string[] { "You should explore the console first." },
                "Stork",
                () =>
                {
                    StartCoroutine(UnlockAfterDelay(2f)); // wait 2 seconds before allowing re-interact
                    if (pelargosAnimator != null)
                        pelargosAnimator.SetBool(interactBool, false);
                }
            );
            return;
        }




        hasBeenInteracted = true;
        isCurrentlyInDialogue = true;

        // ✅ trigger appropriate ending
        if (GameStateManager.Instance.refusedFeathers ||
            GameStateManager.Instance.feathersCollected == 0)
        {
            ShowNoFeatherEnding();
        }
        else
        {
            ShowEndingChoices();
        }
    }
    
    private System.Collections.IEnumerator UnlockAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isCurrentlyInDialogue = false;
    }


    void ShowNoFeatherEnding()
    {
        GameStateManager.Instance.endingChosen = 3;

        DialogueManager.Instance.StartDialogue(
            automaticEndingDialogue,
            "Stork",
            OnDialogueFinished // will reset state after complete
        );

        TriggerFlight();
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
            case 0: // Bouquet
                DialogueManager.Instance.StartDialogue(
                    ending1Dialogue,
                    "Stork",
                    OnDialogueFinished
                );
                break;

            case 1: // Sand
                if (sandBurialCamera != null)
                {
                    sandBurialCamera.TriggerSequence();
                }
                else if (skyTextObject != null)
                {
                    var skyText = skyTextObject.GetComponent<SkyText>();
                    if (skyText) skyText.ShowText(skyMessage);
                }

                Invoke(nameof(OnDialogueFinished), delayBeforeCredits);
                break;

            case 2: // Pierce ears
                DialogueManager.Instance.StartDialogue(
                    ending3Dialogue,
                    "Stork",
                    OnDialogueFinished
                );
                break;
        }

        TriggerFlight();
    }

    void TriggerFlight()
    {
        if (pelargosAnimator != null)
            pelargosAnimator.SetBool(flightBool, true);

        if (flightScript != null)
            flightScript.StartFlying();
    }

    void OnDialogueFinished()
    {
        Debug.Log("EndingNPCDialogue: Dialogue finished.");
        isCurrentlyInDialogue = false; // ✅ unlock
        Invoke(nameof(LoadCredits), delayBeforeCredits);
    }

    void LoadCredits()
    {
        GameStateManager.Instance.LoadCredits();
    }
}
