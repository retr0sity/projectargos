using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton instance of ScoreManager

    public TextMeshProUGUI scoreText; // UI element to display the score
    public int score = 0; // The current score value
    public ProjectileBehavior projectileBehavior; // Reference to the ProjectileBehavior script to adjust projectile speed

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Ensure only one instance of ScoreManager exists (Singleton pattern)
        if (instance == null)
        {
            instance = this; // Set the static instance
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Display the initial score in the UI
        scoreText.text = "SCORE: " + score.ToString();
    }

    // Method to add score
    public void AddScore()
    {
        score++; // Increment the score
        scoreText.text = "SCORE: " + score.ToString(); // Update the score display

        // Every time the score reaches a multiple of 5 (excluding 0), increase the projectile speed by 1
        if (score % 5 == 0 && score != 0)
        {
            projectileBehavior.Speed += 1f; // Increase projectile speed
        }
    }
}
