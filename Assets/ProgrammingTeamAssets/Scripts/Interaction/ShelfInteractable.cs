using UnityEngine;

/// <summary>
/// Dialogue-driven shelf interaction.
/// Each shelf offers one product at a time using the shared dialogue choice UI.
/// </summary>
public class ShelfInteractable : MonoBehaviour
{
    [Header("Products on this shelf")]
    public ProductData[] products;

    [Header("Daily price variance (0 = fixed price)")]
    [Range(0f, 0.5f)]
    public float priceVariance = 0.2f;

    [Header("Dialogue")]
    [SerializeField] private string purchasePromptFormat = "Do you want to buy {0} for ${1:F2}?";

    private bool isPromptActive;

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
        if (isPromptActive)
            return;

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }

        if (CartManager.Instance == null)
        {
            Debug.LogError("CartManager not found!");
            return;
        }

        ProductData product = GetOfferedProduct();
        if (product == null)
        {
            Debug.LogWarning($"[Shelf] No product configured on {gameObject.name}");
            return;
        }

        isPromptActive = true;

        string prompt = string.Format(
            purchasePromptFormat,
            product.productName,
            product.dailyPrice);

        DialogueManager.Instance.ShowChoices(
            new[] { "Yes", "No" },
            choice => OnPurchaseChoice(product, choice),
            prompt);
    }

    ProductData GetOfferedProduct()
    {
        if (products == null || products.Length == 0)
            return null;

        if (products.Length > 1)
            Debug.LogWarning($"[Shelf] {gameObject.name} has multiple products; using the first configured product.");

        return products[0];
    }

    void OnPurchaseChoice(ProductData product, int choiceIndex)
    {
        isPromptActive = false;

        if (choiceIndex != 0)
            return;

        if (CartManager.Instance == null)
        {
            Debug.LogError("CartManager not found!");
            return;
        }

        CartManager.Instance.AddItem(product);
    }
}
