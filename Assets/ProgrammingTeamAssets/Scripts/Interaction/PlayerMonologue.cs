using System.Collections;
using UnityEngine;
using TMPro;
using Core.Managers;

public class PlayerMonologue : MonoBehaviour
{
    public static PlayerMonologue Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private GameObject monologuePanel; // Can be same as dialogue panel
    [SerializeField] private TextMeshProUGUI monologueText;
    [SerializeField] private float displayTime = 4f;
    [SerializeField] private float textSpeed = 0.03f;
    
    private Coroutine currentMonologue;
    private bool isShowingMonologue = false;
    private string currentText;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        // Subscribe to interact event for skipping
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent += OnInteractPressed;
        }
    }
    
    public void ShowMonologue(string text)
    {
        // Stop any existing monologue
        if (currentMonologue != null)
        {
            StopCoroutine(currentMonologue);
        }
        
        // Start new monologue
        currentMonologue = StartCoroutine(DisplayMonologue(text));
    }
    
    public void ShowMonologue(string[] lines)
    {
        // Join lines with newlines
        string fullText = string.Join("\n", lines);
        ShowMonologue(fullText);
    }
    
    IEnumerator DisplayMonologue(string text)
    {
        isShowingMonologue = true;
        currentText = text;
        
        // Enable panel
        if (monologuePanel != null)
            monologuePanel.SetActive(true);
        
        // Type out text
        monologueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            monologueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        
        // Wait for display time
        yield return new WaitForSeconds(displayTime);
        
        // Hide monologue
        HideMonologue();
    }
    
    void OnInteractPressed()
    {
        if (isShowingMonologue)
        {
            // Skip/dismiss current monologue
            if (currentMonologue != null)
            {
                StopCoroutine(currentMonologue);
            }
            HideMonologue();
        }
    }
    
    public void HideMonologue()
    {
        isShowingMonologue = false;
        
        if (monologuePanel != null)
            monologuePanel.SetActive(false);
            
        currentMonologue = null;
    }
    
    public bool IsMonologueActive()
    {
        return isShowingMonologue;
    }
    
    void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractEvent -= OnInteractPressed;
        }
    }
}