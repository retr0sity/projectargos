using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One Canvas panel in the market scene.
/// Shows the product name, base price, daily price, and an Add / Close button.
/// </summary>
public class ShelfUI : MonoBehaviour
{
    public static ShelfUI Instance { get; private set; }

    [Header("Panel root — assign the panel GameObject")]
    public GameObject panel;

    [Header("Text fields")]
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI basePriceText;
    public TextMeshProUGUI dailyPriceText;

    [Header("Buttons")]
    public Button addToCartButton;
    public Button closeButton;

    ProductData _current;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        addToCartButton.onClick.AddListener(OnAddToCart);
        closeButton.onClick.AddListener(Close);

        panel.SetActive(false);
    }

    public void Open(ProductData product)
    {
        _current = product;

        productNameText.text = product.productName;
        basePriceText.text   = $"Normal price: ${product.basePrice:F2}";
        dailyPriceText.text  = $"Today: ${product.dailyPrice:F2}";

        panel.SetActive(true);
    }

    void OnAddToCart()
    {
        if (_current == null) return;

        if (CartManager.Instance == null)
        {
            Debug.LogError("CartManager not found! Make sure it exists in the scene.");
            return;
        }

        CartManager.Instance.AddItem(_current);
        Close();
    }

    public void Close()
    {
        panel.SetActive(false);
        _current = null;
    }
}
