using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds items the player has actually purchased. Persists across scenes.
/// Add to a persistent GameObject (or let it self-create).
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [System.Serializable]
    public class InventoryItem
    {
        public string productName;
        public float  pricePaid;

        public InventoryItem(string name, float price)
        {
            productName = name;
            pricePaid   = price;
        }
    }

    public List<InventoryItem> items = new List<InventoryItem>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called by checkout — moves everything from cart into inventory.
    /// </summary>
    public void AddFromCart()
    {
        if (CartManager.Instance == null) return;

        foreach (var cartItem in CartManager.Instance.items)
        {
            Debug.Log($"[Inventory] {cartItem.productName} — paid ${cartItem.price:F2}");
            items.Add(new InventoryItem(cartItem.productName, cartItem.price));
        }
            

        Debug.Log($"[Inventory] Now holds {items.Count} item(s).");
        CartManager.Instance.Clear();
    }

    public void LogInventory()
    {
        foreach (var item in items)
            Debug.Log($"[Inventory] {item.productName} — paid ${item.pricePaid:F2}");
    }
}
