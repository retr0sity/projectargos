using UnityEngine;
using Core.Managers;

/// <summary>
/// Detects nearby interactable objects and triggers their interaction when player presses interact.
/// Attach to Player GameObject. Objects must be tagged "Interactable" and have OnInteract() method.
/// </summary>
public class InteractionDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionRange = 2f;
    
    private GameObject currentInteractable;
    private bool inputSubscribed = false;

    // ============================================
    // LIFECYCLE
    // ============================================

    void OnEnable()
    {
        TrySubscribeToInput();
    }

    void OnDisable()
    {
        UnsubscribeFromInput();
    }

    void Start()
    {
        TrySubscribeToInput();
    }
    
    void Update()
    {
        // Ensure we're subscribed (in case InputManager wasn't ready at Start)
        TrySubscribeToInput();
        
        // Detect closest interactable in range
        DetectNearbyInteractables();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromInput();
    }

    // ============================================
    // INPUT SUBSCRIPTION
    // ============================================

    void TrySubscribeToInput()
    {
        if (!inputSubscribed && InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent += OnInteractPressed;
            inputSubscribed = true;
        }
    }

    void UnsubscribeFromInput()
    {
        if (inputSubscribed && InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent -= OnInteractPressed;
            inputSubscribed = false;
        }
    }

    // ============================================
    // DETECTION
    // ============================================

    /// <summary>
    /// Find the closest interactable object within range
    /// </summary>
    void DetectNearbyInteractables()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        
        GameObject closest = null;
        float closestDistance = float.MaxValue;
        
        // Find closest interactable in range
        foreach (GameObject obj in interactables)
        {
            if (obj == null) continue;
            
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            
            if (distance <= interactionRange && distance < closestDistance)
            {
                closest = obj;
                closestDistance = distance;
            }
        }
        
        // Update current interactable and prompt
        if (closest != currentInteractable)
        {
            currentInteractable = closest;
            UpdateInteractionPrompt();
        }
    }

    /// <summary>
    /// Show or hide the interaction prompt based on whether an interactable is in range
    /// </summary>
    void UpdateInteractionPrompt()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowInteractionPrompt(currentInteractable != null);
        }
    }

    // ============================================
    // INTERACTION
    // ============================================

    /// <summary>
    /// Called when player presses the interact button
    /// </summary>
    void OnInteractPressed()
    {
        // Don't start new interactions if UI is already active
        // (DialogueManager handles advancing its own UI through its own subscription)
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsAnyUIActive())
            return;
            
        // Trigger interaction on the current interactable object
        if (currentInteractable != null)
        {
            currentInteractable.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
        }
    }

    // ============================================
    // DEBUG
    // ============================================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}