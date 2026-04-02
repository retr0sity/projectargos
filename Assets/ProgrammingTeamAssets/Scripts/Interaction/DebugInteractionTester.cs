using UnityEngine;

// Optional script for testing interactions quickly
public class DebugInteractionTester : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private KeyCode testKey = KeyCode.T;
    [SerializeField] private bool showDebugInfo = true;
    
    void Update()
    {
        // Quick test keys
        if (Input.GetKeyDown(testKey))
        {
            TestCurrentState();
        }
        
        // Reset state with R key
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
        {
            ResetGameState();
        }
    }
    
    void TestCurrentState()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("No GameStateManager found!");
            return;
        }
        
        var state = GameStateManager.Instance;
        Debug.Log("=== GAME STATE ===");
        Debug.Log($"Feathers Collected: {state.feathersCollected}/3");
        Debug.Log($"Refused Feathers: {state.refusedFeathers}");
        Debug.Log($"Visited Other Scene: {state.hasVisitedOtherScene}");
        Debug.Log($"Ending Chosen: {state.endingChosen}");
        Debug.Log($"Return Scene: {state.returnSceneName}");
        Debug.Log($"Return Position: {state.returnPosition}");
        Debug.Log("==================");
    }
    
    void ResetGameState()
    {
    if (GameStateManager.Instance != null)
    {
        GameStateManager.Instance.ResetAllState();
        Debug.Log("Game State Reset!");
    }
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        if (GameStateManager.Instance != null)
        {
            var state = GameStateManager.Instance;
            
            GUI.Box(new Rect(10, 10, 200, 120), "Game State");
            GUI.Label(new Rect(20, 30, 180, 20), $"Feathers: {state.feathersCollected}/3");
            GUI.Label(new Rect(20, 50, 180, 20), $"Refused: {state.refusedFeathers}");
            GUI.Label(new Rect(20, 70, 180, 20), $"Visited Other: {state.hasVisitedOtherScene}");
            GUI.Label(new Rect(20, 90, 180, 20), $"Ending: {state.endingChosen}");
            
            if (GUI.Button(new Rect(20, 110, 80, 20), "Test State"))
            {
                TestCurrentState();
            }
            
            if (GUI.Button(new Rect(110, 110, 80, 20), "Reset"))
            {
                ResetGameState();
            }
        }
    }
}