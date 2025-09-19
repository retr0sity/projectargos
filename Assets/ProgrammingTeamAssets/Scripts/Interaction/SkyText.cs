using System.Collections;
using UnityEngine;
using TMPro;

public class SkyText : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private float fadeOutTime = 1f;
    [SerializeField] private bool startHidden = true;
    
    private Coroutine currentDisplay;
    
    void Start()
    {
        if (startHidden && textComponent != null)
        {
            Color c = textComponent.color;
            c.a = 0;
            textComponent.color = c;
        }
    }
    
    public void ShowText(string text)
    {
        if (currentDisplay != null)
            StopCoroutine(currentDisplay);
            
        currentDisplay = StartCoroutine(DisplayText(text));
    }
    
    IEnumerator DisplayText(string text)
    {
        if (textComponent == null) yield break;
        
        textComponent.text = text;
        
        // Fade in
        float elapsed = 0;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            Color c = textComponent.color;
            c.a = Mathf.Lerp(0, 1, elapsed / fadeInTime);
            textComponent.color = c;
            yield return null;
        }
        
        // Display
        yield return new WaitForSeconds(displayTime);
        
        // Fade out
        elapsed = 0;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            Color c = textComponent.color;
            c.a = Mathf.Lerp(1, 0, elapsed / fadeOutTime);
            textComponent.color = c;
            yield return null;
        }
        
        currentDisplay = null;
    }
    
    public void HideText()
    {
        if (currentDisplay != null)
        {
            StopCoroutine(currentDisplay);
            currentDisplay = null;
        }
        
        if (textComponent != null)
        {
            Color c = textComponent.color;
            c.a = 0;
            textComponent.color = c;
        }
    }
}