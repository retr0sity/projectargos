using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One Canvas panel in the market scene.
/// Shows product name only — price is not displayed here.
/// Supports cycling through multiple products on one shelf.
/// </summary>
public class ShelfUI : MonoBehaviour
{
    public static ShelfUI Instance { get; private set; }

    [Header("Panel root")]
    public GameObject panel;

    [Header("Text fields")]
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI cycleHint; // optional: "< 1 / 2 >"

    [Header("Buttons")]
    public Button addToCartButton;
    public Button closeButton;
    public Button nextButton;     // optional
    public Button previousButton; // optional

    ProductData[] _products;
    int _index;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        addToCartButton.onClick.AddListener(OnAddToCart);
        closeButton.onClick.AddListener(Close);
        if (nextButton != null)     nextButton.onClick.AddListener(Next);
        if (previousButton != null) previousButton.onClick.AddListener(Previous);

        panel.SetActive(false);
    }

    public void Open(ProductData[] products)
    {
        Debug.Log($"[ShelfUI] Opening panel for {products[0].productName}");
        _products = products;
        _index    = 0;
        panel.SetActive(true);
        Refresh();
    }

    void Refresh()
    {
        productNameText.text = _products[_index].productName;

        if (cycleHint != null)
            cycleHint.text = _products.Length > 1 ? $"< {_index + 1} / {_products.Length} >" : "";
    }

    public void Next()
    {
        _index = (_index + 1) % _products.Length;
        Refresh();
    }

    public void Previous()
    {
        _index = (_index - 1 + _products.Length) % _products.Length;
        Refresh();
    }

    void OnAddToCart()
    {
        if (_products == null) return;

        if (CartManager.Instance == null)
        {
            Debug.LogError("CartManager not found!");
            return;
        }

        CartManager.Instance.AddItem(_products[_index]);
        Close();
    }

    public void Close()
    {
        panel.SetActive(false);
        _products = null;
    }
}
