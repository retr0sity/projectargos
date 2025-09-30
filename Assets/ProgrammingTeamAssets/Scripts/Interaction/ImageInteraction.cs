using UnityEngine;

/// <summary>
/// Shows a sprite image when player interacts with this object.
/// Image displays for specified duration OR until player presses interact to dismiss.
/// Can be reopened multiple times (fully reusable).
/// Requires "Interactable" tag and DialogueManager in scene.
/// FIXED: Better state management to prevent double-triggering
/// </summary>
public class ImageInteraction : MonoBehaviour
{
    [Header("Image Settings")]
    [SerializeField] private Sprite imageToShow;
    [SerializeField] private float displayDuration = 4f;
    
    private bool isCurrentlyShowing = false; // FIX: Prevent double-trigger
    
    void Awake()
    {
        if (gameObject.tag == "Untagged")
            gameObject.tag = "Interactable";
    }

    /// <summary>
    /// Called by InteractionDetector when player presses interact
    /// FIX: Added check to prevent double-triggering
    /// </summary>
    public void OnInteract()
    {
        // FIX: Don't trigger if already showing
        if (isCurrentlyShowing) return;
        
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }
        
        if (imageToShow == null)
        {
            Debug.LogWarning($"No sprite assigned to {gameObject.name}");
            return;
        }
        
        isCurrentlyShowing = true;
        
        // Show image (player frozen, auto-close after duration OR manual dismiss with interact)
        DialogueManager.Instance.ShowImage(imageToShow, displayDuration);
        
        // Reset state after a delay to allow the image system to take over
        Invoke("ResetShowingState", 0.5f);
    }
    
    void ResetShowingState()
    {
        isCurrentlyShowing = false;
    }
}