using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    
    [Header("Game State - Debug View")]
	public int feathersCollected = 0;
	public bool refusedFeathers = false;
	public bool hasVisitedOtherScene = false;
	public int endingChosen = 0;
	public bool hasUsedPortal = false; // NEW: Track single portal usage
	public bool fogActive = true; // NEW: Track fog state across scenes

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
        returnPosition = position;
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
}