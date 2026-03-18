using UnityEngine;

/// <summary>
/// Attach to each shelf GameObject, tag it "Interactable".
/// InteractionDetector will call OnInteract() automatically.
/// </summary>
public class ShelfInteractable : MonoBehaviour
{
    [Header("Products on this shelf")]
    public ProductData[] products;

    [Header("Daily price variance (0 = fixed price)")]
    [Range(0f, 0.5f)]
    public float priceVariance = 0.2f;

    void Start()
    {
        if (gameObject.tag != "Interactable")
            gameObject.tag = "Interactable";

        // Randomize daily price for each product once on scene load
        foreach (var p in products)
        {
            float variance = p.basePrice * priceVariance;
            p.dailyPrice   = p.basePrice + Random.Range(-variance, variance);
            p.dailyPrice   = Mathf.Round(p.dailyPrice * 100f) / 100f;
        }
    }

    public void OnInteract()
    {
        Debug.Log($"[Shelf] OnInteract called on {gameObject.name}");
        if (ShelfUI.Instance == null)
        {
            Debug.LogError("ShelfUI not found in scene!");
            return;
        }

        ShelfUI.Instance.Open(products);
    }
}
