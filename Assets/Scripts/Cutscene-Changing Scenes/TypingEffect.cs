using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro

//Lefteris

public class TypingEffect : MonoBehaviour
{
    public TMP_Text textMeshPro; // Reference to the TextMeshPro component
    public List<string> texts; // List of strings to store the 7 different texts
    public float typingSpeed = 0.1f; // Speed of typing in seconds

    private int currentTextIndex = 0; // Index to track the current text in the list
    private Coroutine typingCoroutine; // Reference to the current typing coroutine
   
    private string sequence = "survive";
    private int currentStep = 0;
    private void Start()
    {
        if (texts.Count > 0)
        {
            textMeshPro.text = string.Empty; // Clear the text box initially
            typingCoroutine = StartCoroutine(TypeText()); // Start typing animation with the first text
        }
    }

    public void NextText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Stop the current typing coroutine
        }
        currentTextIndex++;
        if (currentTextIndex >= texts.Count)
        {
            currentTextIndex = 0; // Loop back to the first text if at the end
        }
        typingCoroutine = StartCoroutine(TypeText());
    }

    public void ResetText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Stop the current typing coroutine
        }
        currentTextIndex = 0;
        typingCoroutine = StartCoroutine(TypeText());
    }

    // Coroutine to simulate typing effect
    IEnumerator TypeText()
    {
        // Loop through all the texts in the list (7 in your case)
        while (currentTextIndex < texts.Count)
        {
            string fullText = texts[currentTextIndex]; // Get the current text to type
            textMeshPro.text = string.Empty; // Clear the text box for the new text
            bool insideTag = false;
            string displayedText = "";

            // Type the current text letter by letter
            foreach (char letter in fullText)
            {
                if (letter == '<')
                {
                    insideTag = true;
                }
                
                if (insideTag) {
                    displayedText += letter;
                    if (letter == '>')
                    {
                        insideTag = false;
                        textMeshPro.text += displayedText;
                    }
                } else {
                    displayedText += letter;
                    textMeshPro.text = displayedText; // Append each letter to the text
                    yield return new WaitForSeconds(typingSpeed); // Wait for the specified duration
                }
            }
            yield break;
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(sequence[currentStep].ToString()))
            {
                currentStep++;
                if (currentStep >= sequence.Length)
                {
                    // Sequence completed
                    Debug.Log("Sequence completed!");
                    return;
                }
                NextText(); // Move to the next text in the list
            }
            else
            {
                // Wrong key pressed
                Debug.Log("Wrong key pressed!");
                currentStep = 0;
                ResetText(); // Reset the text to the initial state
            }
        }
    }
}