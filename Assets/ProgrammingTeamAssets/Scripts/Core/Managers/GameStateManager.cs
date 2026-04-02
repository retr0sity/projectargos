using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private const string MainMenuSceneName = "00_Main Menu 3.0";
    
    [Header("Game State - Debug View")]
    public int feathersCollected = 0;
    public bool refusedFeathers = false;
    public bool hasVisitedOtherScene = false;
    public int endingChosen = 0;
    public bool fogActive = true;
    public bool hasReturnedToAlphaStartOnce = false;
    
    // Portal tracking
    private HashSet<string> usedPortals = new HashSet<string>();
    // Add with the other private collections
    private HashSet<string> firedTriggers = new HashSet<string>();

    public bool HasTriggerFired(string triggerID) => firedTriggers.Contains(triggerID);

    public void MarkTriggerFired(string triggerID)
    {
        firedTriggers.Add(triggerID);
        Debug.Log($"[GSM] Trigger '{triggerID}' marked as fired.");
    }

    public void ResetTrigger(string triggerID)
    {
        firedTriggers.Remove(triggerID);
    }
    
    // Return point stack — FILO
    private Stack<ReturnPoint> returnStack = new Stack<ReturnPoint>();

    // Keep these public fields so existing code that reads them directly doesn't break
    public string returnSceneName => returnStack.Count > 0 ? returnStack.Peek().sceneName : "";
    public Vector3 returnPosition => returnStack.Count > 0 ? returnStack.Peek().position : Vector3.zero;

    [System.Serializable]
    private struct ReturnPoint
    {
        public string sceneName;
        public Vector3 position;
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded -= OnReturnSceneLoaded;
            Instance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MainMenuSceneName)
            ResetAllState();
    }

    // ── Feathers ───────────────────────────────────────────────────────

    public void CollectFeather()
    {
        feathersCollected++;
        Debug.Log($"Feathers collected: {feathersCollected}/3");
    }

    public void RefuseFeather()
    {
        refusedFeathers = true;
        Debug.Log("Player refused to collect feathers");
        FeatherPickup[] allFeathers = FindObjectsOfType<FeatherPickup>();
        foreach (var feather in allFeathers)
            if (feather != null) Destroy(feather.gameObject);
    }

    // ── Return stack ───────────────────────────────────────────────────

    /// <summary>
    /// Push a new return point onto the stack.
    /// Called by ScenePortal when saveReturnPoint is true.
    /// </summary>
    public void SetReturnPoint(string sceneName, Vector3 position)
    {
        returnStack.Push(new ReturnPoint { sceneName = sceneName, position = position });
        hasVisitedOtherScene = true;
        Debug.Log($"[GSM] Pushed return point — {sceneName} @ {position}. Stack depth: {returnStack.Count}");
    }

    /// <summary>
    /// Pop the top return point and travel there.
    /// </summary>
    public void ReturnToSavedPosition()
    {
        if (returnStack.Count == 0)
        {
            Debug.LogWarning("[GSM] ReturnToSavedPosition called but stack is empty!");
            return;
        }

        SceneManager.sceneLoaded += OnReturnSceneLoaded;
        SceneManager.LoadScene(returnStack.Peek().sceneName);
    }

    void OnReturnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnReturnSceneLoaded;

        if (returnStack.Count == 0) return;

        ReturnPoint point = returnStack.Pop(); // consume it now that we've arrived

        if (scene.name == "Alpha_StartScene" && !hasReturnedToAlphaStartOnce)
            hasReturnedToAlphaStartOnce = true;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && point.position != Vector3.zero)
            player.transform.position = point.position;

        Debug.Log($"[GSM] Popped return point — arrived at {scene.name}. Stack depth: {returnStack.Count}");
    }

    // ── Misc ───────────────────────────────────────────────────────────

    public void LoadCredits()
    {
        SceneManager.LoadScene("09_Credits");
    }

    public bool HasPortalBeenUsed(string portalID) => usedPortals.Contains(portalID);

    public void MarkPortalAsUsed(string portalID)
    {
        usedPortals.Add(portalID);
        Debug.Log($"Portal '{portalID}' marked as used");
    }

    public void ResetPortal(string portalID)
    {
        usedPortals.Remove(portalID);
        Debug.Log($"Portal '{portalID}' reset");
    }

    public void ResetAllState()
    {
        feathersCollected = 0;
        refusedFeathers = false;
        hasVisitedOtherScene = false;
        endingChosen = 0;
        fogActive = true;
        hasReturnedToAlphaStartOnce = false;

        usedPortals.Clear();
        returnStack.Clear(); // wipe the whole stack on reset

        SceneManager.sceneLoaded -= OnReturnSceneLoaded;
        HungerManager.Instance?.ResetHunger();
        firedTriggers.Clear();

        Debug.Log("[GSM] Global state reset for main menu.");
        badEndingTimerActive = false;
        badEndingTimeRemaining = 0f;
    }
    // Add with other fields
    [Header("Bad Ending Timer")]
    public bool badEndingTimerActive = false;
    public float badEndingTimeRemaining = 0f;

    public void StartBadEndingTimer(float seconds)
    {
        badEndingTimeRemaining = seconds;
        badEndingTimerActive = true;
        Debug.Log($"[GSM] Bad ending timer started — {seconds}s");
    }

    public void StopBadEndingTimer()
    {
        badEndingTimerActive = false;
        badEndingTimeRemaining = 0f;
        Debug.Log("[GSM] Bad ending timer stopped.");
    }

    // Add with other collections
    private HashSet<string> learnedRecipes = new HashSet<string>();

    public bool IsRecipeLearned(string recipeName) => learnedRecipes.Contains(recipeName);

    public void LearnRecipe(string recipeName)
    {
        learnedRecipes.Add(recipeName);
        Debug.Log($"[GSM] Recipe learned: {recipeName}");
    }
}