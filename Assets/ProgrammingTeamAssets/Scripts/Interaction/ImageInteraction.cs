using UnityEngine;

/// <summary>
/// Shows a sprite image when player interacts with this object.
/// Image displays for 4 seconds OR until player presses interact to dismiss.
/// Can be reopened multiple times (fully reusable).
/// Requires "Interactable" tag and DialogueManager in scene.
/// </summary>
public class ImageInteraction : MonoBehaviour
{
    [Header("Image Settings")]
    [SerializeField] private Sprite imageToShow;
    [SerializeField] private float displayDuration = 4f;
    
    void Awake()
    {
        // Ensure this is interactable
        if (gameObject.tag == "Untagged")
            gameObject.tag = "Interactable";
    }

    /// <summary>
    /// Called by InteractionDetector when player presses interact
    /// </summary>
    public void OnInteract()
    {
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
        
        // Show image (player frozen, auto-close after 4 seconds OR manual dismiss with interact)
        DialogueManager.Instance.ShowImage(imageToShow, displayDuration);
    }
}