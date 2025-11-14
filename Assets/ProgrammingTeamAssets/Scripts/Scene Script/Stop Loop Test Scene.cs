using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stopLoop : MonoBehaviour
{
    private AudioSource audioSource; // The AudioSource component to control audio playback

    // Start is called before the first frame update
    void Start()
    {
        // Attempt to find the AudioSource component on the main camera
        audioSource = Camera.main.GetComponent<AudioSource>();
        
        // If the AudioSource component is not found, log an error
        if (audioSource == null)
        {
            Debug.LogError("AudioSource not found on the main camera.");
        }
    }

    // This method is called when another object enters the trigger collider of this object
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger detected with: " + other.gameObject.name); // Log which object triggered the event

        // Check if the object that triggered the event has the "Player" tag
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Trigger with Player detected."); // Log if the player triggered the event

            // Ensure the AudioSource component exists before attempting to modify it
            if (audioSource != null)
            {
                // Stop the audio from looping
                audioSource.loop = false;
                Debug.Log("Audio loop stopped."); // Log confirmation that the loop was stopped
            }
            else
            {
                // Log an error if the AudioSource is null
                Debug.LogError("AudioSource is null.");
            }
        }
    }
}
