using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro

public class CaveTypingEffect : MonoBehaviour
{
    public TMP_Text textMeshPro; // Reference to the TextMeshPro component
    public List<string> texts; // List of strings to store the 7 different texts
    public float typingSpeed = 0.1f; // Speed of typing in seconds

    private int currentTextIndex = 0; // Index to track the current text in the list
    private Coroutine typingCoroutine; // Reference to the current typing coroutine
    private void Start()
    {
        if (texts.Count > 0)
        {
            textMeshPro.text = string.Empty; // Clear the text box initially
            typingCoroutine = StartCoroutine(TypeText()); // Start typing animation with the first text
        }
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

}
