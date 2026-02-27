using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using Core.Managers;

public class CutsceneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera cutsceneCamera;
    public PlayableDirector playableDirector;
    public Animator playerAnimator;          // Reference to the Animator
    public Rigidbody2D playerRigidbody;      // Reference to the player's Rigidbody2D
    public AudioSource mainCameraAudioSource; // Reference to the AudioSource on the main camera
    private bool hasTriggered = false;
    private bool controlsLockedByCutscene = false;
    private float originalLinearDamping = 0f;
    private bool hasCachedDamping = false;

    private void OnEnable()
    {
        if (playableDirector != null)
            playableDirector.stopped += OnCutsceneFinished;
    }

    private void OnDisable()
    {
        if (playableDirector != null)
            playableDirector.stopped -= OnCutsceneFinished;

        // Safety: if this trigger is disabled/destroyed during a scene change,
        // make sure controls are not left locked for the next scene.
        RestorePlayerMovement();
    }

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
        if (InputManager.Instance != null)
        {
            InputManager.Instance.SetControlLock(true);
            controlsLockedByCutscene = true;
        }

        BasePlayerController player = FindObjectOfType<BasePlayerController>();
        player?.ForceStop();

        // Optional: stop any remaining motion immediately
        if (playerRigidbody != null)
        {
            if (!hasCachedDamping)
            {
                originalLinearDamping = playerRigidbody.linearDamping;
                hasCachedDamping = true;
            }

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
        else
        {
            // No timeline assigned: don't leave player locked.
            RestorePlayerMovement();
        }
    }

    private void OnCutsceneFinished(PlayableDirector director)
    {
        RestorePlayerMovement();
    }

    private void RestorePlayerMovement()
    {
        if (controlsLockedByCutscene && InputManager.Instance != null)
            InputManager.Instance.SetControlLock(false);

        controlsLockedByCutscene = false;

        if (playerRigidbody != null && hasCachedDamping)
            playerRigidbody.linearDamping = originalLinearDamping;
    }
}
