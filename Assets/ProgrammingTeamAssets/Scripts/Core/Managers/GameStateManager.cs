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
    
    // Portal tracking - each portal tracked individually
    private HashSet<string> usedPortals = new HashSet<string>();
    
    // For scene returns
    public string returnSceneName;
    public Vector3 returnPosition;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
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
        {
            ResetAllState();
        }
    }
    
    public void CollectFeather()
    {
        feathersCollected++;
        Debug.Log($"Feathers collected: {feathersCollected}/3");
    }
    
    public void RefuseFeather()
    {
        refusedFeathers = true;
        Debug.Log("Player refused to collect feathers");
        
        // Destroy all feather objects in the current scene so they cannot be picked up later
        FeatherPickup[] allFeathers = FindObjectsOfType<FeatherPickup>();
        foreach (var feather in allFeathers)
        {
            if (feather != null)
                Destroy(feather.gameObject);
        }
    }
    
    public void SetReturnPoint(string sceneName, Vector3 position)
    {
        returnSceneName = sceneName;
        returnPosition = position; // <-- This position is now always used
        hasVisitedOtherScene = true;
        
    }
    
    public void ReturnToSavedPosition()
    {
        if (!string.IsNullOrEmpty(returnSceneName))
        {
            SceneManager.sceneLoaded += OnReturnSceneLoaded;
            SceneManager.LoadScene(returnSceneName);
        }
    }
    
    void OnReturnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnReturnSceneLoaded;
        
        // Mark that we've returned to Alpha_StartScene for the first time
        if (scene.name == "Alpha_StartScene" && !hasReturnedToAlphaStartOnce)
        {
            hasReturnedToAlphaStartOnce = true;
        }
        
        // Position player at return point
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && returnPosition != Vector3.zero)
        {
            player.transform.position = returnPosition;
        }
        
        // Clear return data
        returnSceneName = "";
        returnPosition = Vector3.zero;
    }
    
    public void LoadCredits()
    {
        SceneManager.LoadScene("09_Credits");
    }
    
    // Portal tracking methods
    public bool HasPortalBeenUsed(string portalID)
    {
        return usedPortals.Contains(portalID);
    }
    
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

        returnSceneName = "";
        returnPosition = Vector3.zero;

        // ensure no stale return callback can fire after reset.
        SceneManager.sceneLoaded -= OnReturnSceneLoaded;
        HungerManager.Instance?.ResetHunger();

        Debug.Log("GameStateManager: Global state reset for main menu.");
    }
}
