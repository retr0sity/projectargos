using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    
    [Header("Game State")]
    public int feathersCollected = 0;
    public bool refusedFeathers = false;
    public bool hasVisitedOtherScene = false;
    public int endingChosen = 0; // 1, 2, or 3
    
    [Header("Scene Return Data")]
    public string returnSceneName;
    public Vector3 returnPosition;
    
    void Awake()
    {
        if (Instance != null)
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
        
        // Deactivate all feathers in all scenes
        DeactivateAllFeathers();
    }
    
    void DeactivateAllFeathers()
    {
        // Find all feathers in current scene and deactivate them
        FeatherPickup[] feathers = FindObjectsOfType<FeatherPickup>();
        foreach (FeatherPickup feather in feathers)
        {
            feather.gameObject.SetActive(false);
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
            SceneManager.LoadScene(returnSceneName);
            // Position will be set by checking this in the scene
        }
    }
    
    public void LoadCredits()
    {
        SceneManager.LoadScene("09_Credits");
    }
    
    // Call this from the scene to position player after loading
    public bool TryPositionPlayerAtReturn()
    {
        if (SceneManager.GetActiveScene().name == returnSceneName && returnPosition != Vector3.zero)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = returnPosition;
                // Clear return data
                returnSceneName = "";
                returnPosition = Vector3.zero;
                return true;
            }
        }
        return false;
    }
}