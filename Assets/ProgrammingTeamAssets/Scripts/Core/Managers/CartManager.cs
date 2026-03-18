using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds items the player has selected to buy. Persists across scenes.
/// Add to a persistent GameObject (or let it self-create).
/// </summary>
public class CartManager : MonoBehaviour
{
    public static CartManager Instance { get; private set; }

    [System.Serializable]
    public class CartItem
    {
        public string productName;
        public float  price;

        public CartItem(string name, float price)
        {
            productName = name;
            this.price  = price;
        }
    }

    public List<CartItem> items = new List<CartItem>();
    public float Total { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(ProductData product)
    {
        items.Add(new CartItem(product.productName, product.dailyPrice));
        Total += product.dailyPrice;
        Debug.Log($"[Cart] Added {product.productName} (${product.dailyPrice:F2}) — Total: ${Total:F2}");
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= items.Count) return;
        Total -= items[index].price;
        items.RemoveAt(index);
    }

    public void Clear()
    {
        items.Clear();
        Total = 0f;
    }
}
