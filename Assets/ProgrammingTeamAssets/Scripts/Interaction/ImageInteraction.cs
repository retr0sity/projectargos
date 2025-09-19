using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageInteraction : MonoBehaviour
{
    [Header("Image Settings")]
    [SerializeField] private GameObject imagePopup; // The popup panel
    [SerializeField] private Image displayImage; // The Image component
    [SerializeField] private Sprite imageToShow; // The sprite to display
    [SerializeField] private float displayTime = 3f; // How long to show (0 = press to close)
    
    private bool hasBeenViewed = false;
    private PlayerController playerController;
    
    void Start()
    {
        // Make absolutely sure the image popup is hidden at start
        if (imagePopup != null)
        {
            imagePopup.SetActive(false);
        }
        else
        {
            Debug.LogWarning("ImagePopup not assigned in " + gameObject.name);
        }
            
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerController = player.GetComponent<PlayerController>();
    }
    
    void OnInteract()
    {
        if (hasBeenViewed) return;
        
        hasBeenViewed = true;
        StartCoroutine(ShowImage());
    }
    
    IEnumerator ShowImage()
    {
        // Show the image
        if (imagePopup != null && displayImage != null && imageToShow != null)
        {
            imagePopup.SetActive(true);
            displayImage.sprite = imageToShow;
            
            // Pause player
            if (playerController != null)
                playerController.enabled = false;
            
            if (displayTime > 0)
            {
                // Auto close after time
                yield return new WaitForSeconds(displayTime);
            }
            else
            {
                // Wait for any key press to close
                yield return new WaitForSeconds(0.5f); // Small delay
                yield return new WaitUntil(() => Input.anyKeyDown);
            }
            
            // Hide image and resume
            imagePopup.SetActive(false);
            
            if (playerController != null)
                playerController.enabled = true;
                
            // Remove from interactables
            gameObject.tag = "Untagged";
        }
    }
}