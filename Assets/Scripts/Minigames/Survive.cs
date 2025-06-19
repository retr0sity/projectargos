using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Survive : MonoBehaviour
{
    public GameObject[] gameObjects; // Array to hold the 3 game objects that will change sprites during the game
    public Sprite[] sprites; // Array to hold the sprites for each step of the game sequence
    private string sequence = "survive"; // Sequence of steps that the player needs to follow (keys to press)
    private int currentStep = 0; // The current step in the sequence that the player is on
    private AudioManager audioManager; // Reference to the AudioManager to control audio during gameplay

    // Start is called before the first frame update
    void Start()
    {
        // Start the minigame theme music
        FindObjectOfType<AudioManager>().Play("minigame_theme");
        
        // Find the AudioManager component to manage audio during the game
        audioManager = FindObjectOfType<AudioManager>();

        // Reset the game objects (clear the sprites) at the beginning of the game
        ResetGameObjects();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if any key is pressed
        if (Input.anyKeyDown)
        {
            // Check if the correct key (matching the current step in the sequence) is pressed
            if (Input.GetKeyDown(sequence[currentStep].ToString()))
            {
                // Update the game objects (change their sprites)
                UpdateGameObjects();

                // Move to the next step in the sequence
                currentStep++;

                // Adjust the pitch of the music based on the current step
                if (audioManager != null)
                {
                    audioManager.AdjustPitch("minigame_theme", 1.0f + currentStep * 0.1f);
                }

                // If the sequence is completed, trigger the completion logic
                if (currentStep >= sequence.Length)
                {
                    Debug.Log("Sequence completed!");
                    StartCoroutine(Completion(4.0f)); // Wait 4 seconds before finishing
                }
            }
            else
            {
                // If the wrong key is pressed, reset the sequence and game objects
                Debug.Log("Wrong key pressed!");
                currentStep = 0;
                ResetGameObjects();
            }
        }
    }

    // Updates the sprites of the game objects based on the current step in the sequence
    void UpdateGameObjects()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            // Calculate the correct sprite index based on the current step and the number of game objects
            int spriteIndex = currentStep * gameObjects.Length + i;

            // If the sprite index is valid, update the sprite of the game object
            if (spriteIndex < sprites.Length)
            {
                gameObjects[i].GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];
            }
        }
    }

    // Resets the sprites of the game objects to their initial state
    void ResetGameObjects()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            // Apply a specific rule for the game object named "Cable Highlights"
            if (gameObjects[i].name == "Cable Highlights")
            {
                gameObjects[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("1");
            }
            else
            {
                // Reset all other game objects to have no sprite
                gameObjects[i].GetComponent<SpriteRenderer>().sprite = null;
            }
        }

        // Reset the pitch of the minigame theme music to its default value
        if (audioManager != null)
        {
            audioManager.ResetPitch("minigame_theme");
        }
    }

    // Coroutine that runs after the sequence is completed, handles post-sequence actions
    private IEnumerator Completion(float waitTime)
    {
        // Wait for a brief time before starting the next actions (this creates a small delay after sequence completion)
        yield return new WaitForSeconds(waitTime - 0.1f);

        // Play a sound effect when the sequence is completed
        FindObjectOfType<AudioManager>().Play("dying2_sfx");

        // Wait for the sound effect to finish before transitioning to the next scene
        yield return new WaitForSeconds(0.1f);

        // Load the next scene, which is "arduino"
        SceneManager.LoadScene("07_arduino");
    }
}
