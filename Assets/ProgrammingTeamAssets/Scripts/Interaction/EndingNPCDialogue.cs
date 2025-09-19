using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingNPCDialogue : MonoBehaviour
{
    [Header("NPC Settings")]
    [SerializeField] private string npcName = "Pelargos";
    
    [Header("Dialogue - Intro")]
    [SerializeField] private string[] introDialogueWithFeathers;
    [SerializeField] private string[] introDialogueNoFeathers;
    
    [Header("Ending Dialogues")]
    [SerializeField] private string[] ending1Dialogue; // Make bouquet
    [SerializeField] private string[] ending2Dialogue; // Bury in sand
    [SerializeField] private string[] ending3DialogueChosen; // Pierce ears (chosen)
    [SerializeField] private string[] ending3DialogueAutomatic; // Pierce ears (automatic/refused)
    
    [Header("Animations")]
    [SerializeField] private Animator npcAnimator; // Optional for later
    [SerializeField] private string ending1AnimationTrigger = "Ending1";
    [SerializeField] private string ending2AnimationTrigger = "Ending2";
    [SerializeField] private string ending3AnimationTrigger = "Ending3";
    
    private bool hasBeenInteracted = false;
    
    void OnInteract()
    {
        if (hasBeenInteracted) return;
        if (GameStateManager.Instance == null) return;
        
        // Check if player has visited the other scene
        if (!GameStateManager.Instance.hasVisitedOtherScene)
        {
            Debug.Log("Player hasn't visited the other scene yet");
            return;
        }
        
        hasBeenInteracted = true;
        
        // Check if player refused feathers
        if (GameStateManager.Instance.refusedFeathers || GameStateManager.Instance.feathersCollected == 0)
        {
            // Automatic ending 3
            ShowAutomaticEnding3();
        }
        else
        {
            // Show intro then choices
            ShowIntroWithChoices();
        }
    }
    
    void ShowAutomaticEnding3()
    {
        GameStateManager.Instance.endingChosen = 3;
        
        // Show the automatic ending 3 dialogue
        DialogueManager.Instance.StartDialogue(
            ending3DialogueAutomatic, 
            npcName, 
            () => {
                // Play animation if available
                if (npcAnimator != null)
                    npcAnimator.SetTrigger(ending3AnimationTrigger);
                    
                // Wait a bit then load credits
                Invoke("LoadCredits", 2f);
            }
        );
    }
    
    void ShowIntroWithChoices()
    {
        // Show intro dialogue first
        DialogueManager.Instance.StartDialogue(
            introDialogueWithFeathers,
            npcName,
            () => {
                // After intro, show the 3 choices
                ShowEndingChoices();
            }
        );
    }
    
    void ShowEndingChoices()
    {
        string[] choices = new string[] {
            "Make a bouquet and give them",
            "Bury them in the sand",
            "Pierce your ears with the feathers"
        };
        
        DialogueManager.Instance.ShowChoices(choices, OnEndingChosen);
    }
    
    void OnEndingChosen(int choiceIndex)
    {
        GameStateManager.Instance.endingChosen = choiceIndex + 1;
        
        string[] selectedDialogue = null;
        string animationTrigger = "";
        
        switch (choiceIndex)
        {
            case 0: // Ending 1 - Bouquet
                selectedDialogue = ending1Dialogue;
                animationTrigger = ending1AnimationTrigger;
                break;
            case 1: // Ending 2 - Bury
                selectedDialogue = ending2Dialogue;
                animationTrigger = ending2AnimationTrigger;
                break;
            case 2: // Ending 3 - Pierce ears
                selectedDialogue = ending3DialogueChosen;
                animationTrigger = ending3AnimationTrigger;
                break;
        }
        
        // Show ending dialogue
        DialogueManager.Instance.StartDialogue(
            selectedDialogue,
            npcName,
            () => {
                // Play animation if available
                if (npcAnimator != null && !string.IsNullOrEmpty(animationTrigger))
                    npcAnimator.SetTrigger(animationTrigger);
                    
                // Wait then load credits
                Invoke("LoadCredits", 2f);
            }
        );
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