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
        
        hasBeenTalkedTo = true;
        
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueLines, npcName, OnDialogueComplete);
        }
    }
    
    void OnDialogueComplete()
    {
        Debug.Log("NPC dialogue completed");
        // Could disable the interaction here or change state
        gameObject.tag = "Untagged"; // Remove from interactables
    }
}