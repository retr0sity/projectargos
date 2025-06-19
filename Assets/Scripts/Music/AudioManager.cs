using UnityEngine.Audio; // Unity's audio library
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds; // Array of sounds to manage different audio clips

    void Awake()
    {
        // Loop through all sounds and assign properties to their AudioSource components
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>(); // Add an AudioSource component for each sound
            s.source.clip = s.clip; // Assign the audio clip

            s.source.volume = s.volume; // Set volume
            s.source.pitch = s.pitch; // Set pitch

            if (PauseMenu.GameIsPaused)
            {
                s.source.volume *= .5f; // If the game is paused, reduce the volume of all sounds
            }
        }
    }

    // Plays a sound by name
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name); // Find the sound in the array
        if (s != null) // Ensure the sound exists
        {
            s.source.Play(); // Play the sound
        }
    }

    // Adjusts the pitch of a specific sound
    public void AdjustPitch(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.pitch = pitch; // Change the pitch
        }
    }

    // Resets the pitch of a specific sound to its default value (1.0)
    public void ResetPitch(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.pitch = 1.0f; // Reset pitch to normal
        }
    }

    // Stops a sound from playing
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Stop(); // Stop the sound
        }
    }
}
