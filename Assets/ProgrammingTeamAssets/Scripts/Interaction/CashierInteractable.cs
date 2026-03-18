using UnityEngine;

/// <summary>
/// Attach to the cashier GameObject. Tag it "Interactable".
/// InteractionDetector calls OnInteract() automatically.
/// </summary>
public class CashierInteractable : MonoBehaviour
{
    void Start()
    {
        if (gameObject.tag != "Interactable")
            gameObject.tag = "Interactable";
    }

    public void OnInteract()
    {
        if (CartManager.Instance == null || CartManager.Instance.items.Count == 0)
        {
            Debug.Log("[Cashier] Cart is empty.");
            return;
        }

        if (CheckoutUI.Instance == null)
        {
            Debug.LogError("CheckoutUI not found in scene!");
            return;
        }

        CheckoutUI.Instance.OpenPrompt();
    }
}
