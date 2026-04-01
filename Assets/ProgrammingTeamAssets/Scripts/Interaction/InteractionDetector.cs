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
    [Header("Highlight")]
    [SerializeField] private Color highlightColor = new Color(1f, 1f, 0.5f, 1f);
    [SerializeField] private float highlightIntensity = 0.3f;
    private Color _previousColor = Color.white;
    
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
        TrySubscribeToInput();
        
        // FIX: Check if current interactable was destroyed
        if (currentInteractable != null && currentInteractable.Equals(null))
        {
            currentInteractable = null;
            UpdateInteractionPrompt();
        }
        
        DetectNearbyInteractables();
        
        // FIX: Continuously update prompt state based on UI activity
        UpdateInteractionPrompt();
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
        
        if (closest != currentInteractable)
        {
            currentInteractable = closest;
            UpdateInteractionPrompt();
        }
        if (closest != currentInteractable)
        {
            // Remove highlight from previous
            if (currentInteractable != null)
            {
                SpriteRenderer sr = currentInteractable.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = _previousColor;
            }

            currentInteractable = closest;

            // Highlight new
            if (currentInteractable != null)
            {
                SpriteRenderer sr = currentInteractable.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    _previousColor = sr.color;
                    sr.color = Color.Lerp(_previousColor, highlightColor, highlightIntensity);
                }
            }

            UpdateInteractionPrompt();
        }
    }

    /// <summary>
    /// Show or hide the interaction prompt based on whether an interactable is in range
    /// FIX: Also check if UI is active before showing prompt
    /// </summary>
    void UpdateInteractionPrompt()
    {
        if (DialogueManager.Instance != null)
        {
            // Only show prompt if there's an interactable AND no UI is active
            bool shouldShow = currentInteractable != null && !DialogueManager.Instance.IsAnyUIActive();
            DialogueManager.Instance.ShowInteractionPrompt(shouldShow);
        }
    }

    // ============================================
    // INTERACTION
    // ============================================

    /// <summary>
    /// Called when player presses the interact button
    /// FIX: Only processes new interactions, not ongoing UI
    /// </summary>
    void OnInteractPressed()
    {
        Debug.Log($"[Interaction] UI Active: {DialogueManager.Instance.IsAnyUIActive()}, Current interactable: {currentInteractable?.name}");
        // FIX: Don't start new interactions if ANY UI is active
        // DialogueManager handles its own advancement through its own subscription
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsAnyUIActive())
        {
            // Let DialogueManager handle the interact event
            return;
        }
            
        // Trigger interaction on the current interactable object
        if (currentInteractable != null)
        {
            // Send the message to trigger OnInteract()
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