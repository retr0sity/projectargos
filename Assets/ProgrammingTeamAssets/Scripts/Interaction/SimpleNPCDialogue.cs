using UnityEngine;

public class SimpleNPCDialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private string npcName = "NPC";
    [SerializeField] private string[] dialogueLines;
    
    private bool hasBeenTalkedTo = false;
    
    void OnInteract()
    {
        if (hasBeenTalkedTo) return;
        
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found!");
            return;
        }
        
        hasBeenTalkedTo = true;
        
        DialogueManager.Instance.StartDialogue(dialogueLines, npcName, OnDialogueComplete);
    }
    
    void OnDialogueComplete()
    {
        Debug.Log("NPC dialogue completed");
        // Could disable the interaction here or change state
        gameObject.tag = "Untagged"; // Remove from interactables
    }
}