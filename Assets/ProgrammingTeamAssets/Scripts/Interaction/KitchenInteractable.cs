using UnityEngine;

/// <summary>
/// Bridges the scene kitchen object into the Home cooking flow.
/// </summary>
public class KitchenInteractable : MonoBehaviour
{
    private const float InteractCooldown = 0.5f;
    private float lastInteractTime = -999f;

    public void OnInteract()
    {
        if (Time.time - lastInteractTime < InteractCooldown)
            return;

        lastInteractTime = Time.time;

        if (HomeCookingManager.Instance == null)
        {
            Debug.LogError("HomeCookingManager not available for kitchen interaction.");
            return;
        }

        HomeCookingManager.Instance.UseKitchen();
    }
}