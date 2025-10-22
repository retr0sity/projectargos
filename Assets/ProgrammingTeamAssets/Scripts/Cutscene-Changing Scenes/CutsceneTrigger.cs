using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera cutsceneCamera;
    public PlayableDirector playableDirector;
    public Animator playerAnimator;          // Reference to the Animator
    public Rigidbody2D playerRigidbody;      // Reference to the player's Rigidbody2D
    public AudioSource mainCameraAudioSource; // Reference to the AudioSource on the main camera
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player") && !hasTriggered) {
            FindObjectOfType<AudioManager>().Play("dying2");
            Debug.Log("Trigger!");
            hasTriggered = true;
            DisablePlayerMovement();
            SwitchCameras();
            LowerMainCameraAudio();
            Invoke("PlayTimeline", 3);
        }
    }

    private void SwitchCameras()
    {
        if (playerCamera != null && cutsceneCamera != null)
        {
            cutsceneCamera.gameObject.SetActive(true);   // Activate cutsceneCamera
            playerCamera.gameObject.SetActive(false);  // Deactivate playerCamera
        }
    }

    private void DisablePlayerMovement()
    {
        // Find the PlayerController on the player object
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.LockPlayer(); // Call your existing lock method
        }

        // Optional: stop any remaining motion immediately
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.linearDamping = 5f;
        }

        // Make sure animation isn’t stuck in jumping state
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsJumping", false);
        }
    }


    private void LowerMainCameraAudio()
    {
        if (mainCameraAudioSource != null)
        {
            mainCameraAudioSource.volume = 0f; // Lower the volume to 0% of the original
        }
    }

    private void PlayTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play();  // Play the assigned timeline
        }
    }
}