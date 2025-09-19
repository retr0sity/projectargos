using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingNPCDialogue : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private string storkName = "Stork";
    
    [Header("Sky/Sand Text References")]
    [SerializeField] private SkyText skyTextDisplay; // For sky messages
    [SerializeField] private SkyText sandTextDisplay; // For sand messages
    
    [Header("Dialogue - No Feathers Path")]
    [TextArea(3,5)]
    [SerializeField] private string[] automaticEndingDialogue; // "One day you will learn how to use it."
    
    [Header("Choice Texts")]
    [SerializeField] private string choice1Text = "Make a bouquet and give them";
    [SerializeField] private string choice2Text = "Bury them in the sand";
    [SerializeField] private string choice3Text = "Pierce your ears with them";
    
    [Header("Ending 1 - Bouquet")]
    [TextArea(3,5)]
    [SerializeField] private string[] ending1StorkDialogue; // "You're kind. But you need them more than I do."
    
    [Header("Ending 2 - Sand")]
    [TextArea(3,5)]
    [SerializeField] private string sandMessage = "The sea will bring them back when you need them.";
    
    [Header("Ending 3 - Pierce Ears")]
    [TextArea(3,5)]
    [SerializeField] private string[] ending3StorkDialogue; // "You will get them back when you know how to use them."
    
    [Header("Animations")]
    [SerializeField] private Animator storkAnimator;
    [SerializeField] private string ending1Animation = "GiveBouquet";
    [SerializeField] private string ending2Animation = "BuryFeathers";
    [SerializeField] private string ending3Animation = "PierceEars";
    
    [Header("Settings")]
    [SerializeField] private float delayBeforeCredits = 3f;
    
    private bool hasBeenInteracted = false;
    
    void OnInteract()
    {
        if (hasBeenInteracted) return;
        if (GameStateManager.Instance == null) return;
        
        // Check if player has visited the other scene
        if (!GameStateManager.Instance.hasVisitedOtherScene)
        {
            Debug.Log("Player hasn't completed the mini-game yet");
            return;
        }
        
        hasBeenInteracted = true;
        
        // Check if player has any feathers
        if (GameStateManager.Instance.refusedFeathers || GameStateManager.Instance.feathersCollected == 0)
        {
            // Automatic ending - no choices
            ShowNoFeathersEnding();
        }
        else
        {
            // Player has feathers - show choices
            ShowFeatherChoices();
        }
    }
    
    void ShowNoFeathersEnding()
    {
        GameStateManager.Instance.endingChosen = 3; // Mark as ending 3
        
        // Stork takes the feather back and speaks
        DialogueManager.Instance.StartDialogue(
            automaticEndingDialogue, 
            storkName,
            () => {
                PlayAnimation(ending3Animation);
                Invoke("LoadCredits", delayBeforeCredits);
            }
        );
    }
    
    void ShowFeatherChoices()
    {
        // Go straight to choices (no intro dialogue needed)
        string[] choices = new string[] {
            choice1Text,
            choice2Text,
            choice3Text
        };
        
        DialogueManager.Instance.ShowChoices(choices, OnEndingChosen);
    }
    
    void OnEndingChosen(int choiceIndex)
    {
        GameStateManager.Instance.endingChosen = choiceIndex + 1;
        
        switch (choiceIndex)
        {
            case 0: // Ending 1 - Give Bouquet
                HandleBouquetEnding();
                break;
            case 1: // Ending 2 - Bury in Sand
                HandleSandEnding();
                break;
            case 2: // Ending 3 - Pierce Ears
                HandlePierceEarsEnding();
                break;
        }
    }
    
    void HandleBouquetEnding()
    {
        // Stork speaks
        DialogueManager.Instance.StartDialogue(
            ending1StorkDialogue,
            storkName,
            () => {
                PlayAnimation(ending1Animation);
                Invoke("LoadCredits", delayBeforeCredits);
            }
        );
    }
    
    void HandleSandEnding()
    {
        // Show text on sand (not in dialogue box)
        if (sandTextDisplay != null)
        {
            sandTextDisplay.ShowText(sandMessage);
        }
        else
        {
            Debug.LogWarning("Sand text display not assigned - showing in dialogue instead");
            DialogueManager.Instance.StartDialogue(
                new string[] { sandMessage },
                "Sand",
                () => { }
            );
        }
        
        PlayAnimation(ending2Animation);
        Invoke("LoadCredits", delayBeforeCredits);
    }
    
    void HandlePierceEarsEnding()
    {
        // Stork speaks (same as automatic ending)
        DialogueManager.Instance.StartDialogue(
            ending3StorkDialogue,
            storkName,
            () => {
                PlayAnimation(ending3Animation);
                Invoke("LoadCredits", delayBeforeCredits);
            }
        );
    }
    
    void PlayAnimation(string triggerName)
    {
        if (storkAnimator != null && !string.IsNullOrEmpty(triggerName))
        {
            storkAnimator.SetTrigger(triggerName);
        }
    }
    
    void LoadCredits()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.LoadCredits();
        }
        else
        {
            SceneManager.LoadScene("09_Credits");
        }
    }
}