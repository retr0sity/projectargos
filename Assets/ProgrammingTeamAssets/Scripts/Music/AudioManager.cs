using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using MyGame.Audio; // Import the namespace for your Sound class

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(.1f, 3f)] public float pitch = 1f;
    [HideInInspector] public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sounds")]
    public Sound[] sounds;

    [Header("Default Music")]
    public string defaultMusic = "MainTheme"; // Plays in most scenes
    public float fadeDuration = 1f; // Seconds to fade between tracks

    private Sound currentMusic;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize AudioSources
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = true; // All music loops by default
        }

        // Listen to scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Play default music at game start
        PlayMusic(defaultMusic);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Example: switch music per scene
        switch (scene.name)
        {
            case "BossScene":
                PlayMusic("BossTheme");
                break;
            case "MenuScene":
                PlayMusic("MenuTheme");
                break;
            default:
                PlayMusic(defaultMusic);
                break;
            case "MainGame 1":
                PlayMusic("irlTheme");
                break;
            case "MainMenu":
                PlayMusic("irlTheme");
                break;
            case "DeskScene":
                PlayMusic("irlTheme");
                break;
        
            
        }
    }

    /// <summary>
    /// Play a sound effect immediately (non-looping).
    /// </summary>
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.PlayOneShot(s.clip, s.volume);
    }

    /// <summary>
    /// Start playing music with optional fade.
    /// </summary>
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"AudioManager: Music '{name}' not found!");
            return;
        }

        if (currentMusic == s)
            return; // Already playing this track

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeMusic(currentMusic, s));
        currentMusic = s;
    }

    /// <summary>
    /// Fade out old track, fade in new track.
    /// </summary>
    private IEnumerator FadeMusic(Sound from, Sound to)
    {
        float t = 0f;

        // Start new track at 0 volume
        if (to != null)
        {
            to.source.volume = 0f;
            to.source.Play();
        }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float ratio = t / fadeDuration;

            if (from != null)
                from.source.volume = Mathf.Lerp(from.volume, 0f, ratio);
            if (to != null)
                to.source.volume = Mathf.Lerp(0f, to.volume, ratio);

            yield return null;
        }

        if (from != null)
            from.source.Stop();
        if (to != null)
            to.source.volume = to.volume;
    }

    public void StopMusic()
    {
        if (currentMusic != null)
            currentMusic.source.Stop();
    }

    public void AdjustPitch(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.pitch = pitch;
    }

    public void ResetPitch(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.pitch = 1f;
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.Stop();
    }
}