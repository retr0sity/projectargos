using UnityEngine;

/// <summary>
/// Attach to each shelf GameObject.
/// Tag the GameObject as "Interactable" — InteractionDetector will call OnInteract().
/// </summary>
public class ShelfInteractable : MonoBehaviour
{
    [Header("Product on this shelf")]
    public ProductData product = new ProductData("Product", 1.00f);

    [Header("Daily price randomization")]
    [Tooltip("How much the daily price can vary from base (0 = no randomization)")]
    [Range(0f, 0.5f)]
    public float priceVariance = 0.2f;

    void Start()
    {
        // Randomize daily price once on scene load
        float variance  = product.basePrice * priceVariance;
        product.dailyPrice = product.basePrice + Random.Range(-variance, variance);
        product.dailyPrice = Mathf.Round(product.dailyPrice * 100f) / 100f; // 2 decimal places

        // Make sure the tag is set
        if (gameObject.tag != "Interactable")
            gameObject.tag = "Interactable";
    }

    // Called by InteractionDetector via SendMessage
    public void OnInteract()
    {
        if (ShelfUI.Instance == null)
        {
            Debug.LogError("ShelfUI not found in scene!");
            return;
        }

        ShelfUI.Instance.Open(product);
    }
}
