using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ArduinoBehavior : MonoBehaviour
{
    // References
    public Animator playerAnimator; // Controls the player's animations
    public TMP_Text textMeshPro; // Text component to display messages

    // Typing Effect Settings
    public List<string> texts; // List of text messages to display
    public float typingSpeed = 0.1f; // Speed of the typing effect

    private AudioManager audioManager; // Reference to the AudioManager
    private bool isDead = false; // Prevents multiple death triggers
    private int currentTextIndex = 0; // Tracks which text is being displayed

    void Start()
    {
        // Find and play the Arduino background sound
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.Play("arduino");
        }
    }

    public void TakeDamage(int amount)
    {
        // If the death sequence is already running, do nothing
        if (isDead) return;

        isDead = true; // Mark Arduino as dead

        // Stop Arduino's normal sound and play death sounds
        if (audioManager != null)
        {
            audioManager.Stop("arduino");
            audioManager.Play("death_sfx");
            audioManager.Play("arduino_death");
        }

        playerAnimator.enabled = false; // Stop player animations
        Time.timeScale = 0; // Freeze the game

        StartCoroutine(ArduinoDeath()); // Start the death sequence
    }

    private IEnumerator ArduinoDeath()
    {
        // Loop through all texts and display them one at a time
        while (currentTextIndex < texts.Count)
        {
            textMeshPro.text = ""; // Clear the text box
            string fullText = texts[currentTextIndex]; // Get the current message

            // Type each letter one by one
            foreach (char letter in fullText)
            {
                textMeshPro.text += letter;
                yield return new WaitForSecondsRealtime(typingSpeed); // Keep time frozen while typing
            }

            currentTextIndex++; // Move to the next text
            yield return new WaitForSecondsRealtime(1f); // Short delay before the next message
        }

        yield return new WaitForSecondsRealtime(4f); // Wait before switching scenes

        Time.timeScale = 1; // Resume normal game time
        SceneManager.LoadScene("08_cave_afterminigame"); // Load the next scene
    }
}
