using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Managers;

public class InteractionDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private GameObject interactionPrompt; // The "Press E" UI element
    
    private GameObject currentInteractable;
    private GameObject[] interactables;
    
    void Start()
    {
        // Subscribe to input
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent += OnInteractPressed;
        }
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
            
        // Cache all interactables at start
        RefreshInteractables();
    }
    
    void Update()
    {
        CheckForInteractables();
    }
    
    public void RefreshInteractables()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
    }
    
    void CheckForInteractables()
    {
        GameObject closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (GameObject obj in interactables)
        {
            if (obj == null || !obj.activeInHierarchy) continue;
            
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if (distance < interactionRange && distance < closestDistance)
            {
                closest = obj;
                closestDistance = distance;
            }
        }
        
        if (closest != currentInteractable)
        {
            currentInteractable = closest;
            UpdatePrompt();
        }
    }
    
    void UpdatePrompt()
    {
        if (currentInteractable != null && interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
        else if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    void OnInteractPressed()
    {
        if (currentInteractable != null)
        {
            // Check if DialogueManager exists and if dialogue is active
            bool dialogueActive = DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive();
            
            if (!dialogueActive)
            {
                currentInteractable.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    
    void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent -= OnInteractPressed;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}