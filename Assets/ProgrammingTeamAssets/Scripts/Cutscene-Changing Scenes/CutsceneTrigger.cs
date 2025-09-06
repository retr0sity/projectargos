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
    public MonoBehaviour jumpMovementScript;
    public MonoBehaviour horizontalMovementScript;
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
        if (horizontalMovementScript != null)
        {
            horizontalMovementScript.enabled = false;  // Disable horizontal movement
        }

        if (jumpMovementScript != null)
        {
            jumpMovementScript.enabled = false;  // Disable jump movement
        }

        // Freeze the Rigidbody2D (stop any movement or physics)
        if (playerRigidbody != null)
        {
            playerRigidbody.linearDamping = 5f;
            //playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
            //playerRigidbody.velocity = Vector2.zero; // Stop any existing movement
        }

        // Set the animator to a "cutscene" state or stop falling animation
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsJumping", false); // Assuming IsJumping controls jumping animation
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