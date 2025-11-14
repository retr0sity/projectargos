using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; // The name of the sound (used for identification)

    public AudioClip clip; // The audio clip associated with the sound

    [Range(0f, 1f)]
    public float volume; // Volume of the sound, ranging from 0 (muted) to 1 (full volume)

    [Range(.1f, 3f)]
    public float pitch; // Pitch of the sound, where 1 is the default pitch

    [HideInInspector]
    public AudioSource source; // The AudioSource component that plays the sound (hidden in the Inspector)
}
