using UnityEngine;

/// <summary>
/// Bridges the scene fridge object into the Home cooking flow.
/// </summary>
public class FridgeInteractable : MonoBehaviour
{
    public void OnInteract()
    {
        if (HomeCookingManager.Instance == null)
        {
            Debug.LogError("HomeCookingManager not available for fridge interaction.");
            return;
        }

        HomeCookingManager.Instance.OpenFridge();
    }
}
