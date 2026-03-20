using UnityEngine;

/// <summary>
/// Bridges the scene kitchen object into the Home cooking flow.
/// </summary>
public class KitchenInteractable : MonoBehaviour
{
    public void OnInteract()
    {
        if (HomeCookingManager.Instance == null)
        {
            Debug.LogError("HomeCookingManager not available for kitchen interaction.");
            return;
        }

        HomeCookingManager.Instance.UseKitchen();
    }
}
