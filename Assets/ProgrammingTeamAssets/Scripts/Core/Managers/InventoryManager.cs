using System.Collections.Generic;
using UnityEngine;

public enum InventoryItemKind
{
    Ingredient,
    CookedMeal
}

public enum MealQualityTier
{
    Simple,
    Tasty,
    Excellent
}

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
        public InventoryItemKind itemKind = InventoryItemKind.Ingredient;
        public string sourceRecipeName;
        public MealQualityTier qualityTier = MealQualityTier.Simple;
        public string mealName;

        public InventoryItem(string name, float price, InventoryItemKind kind = InventoryItemKind.Ingredient)
        {
            productName      = name;
            pricePaid        = price;
            itemKind         = kind;
            sourceRecipeName = string.Empty;
            qualityTier      = MealQualityTier.Simple;
            mealName         = kind == InventoryItemKind.CookedMeal ? name : string.Empty;
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
            AddIngredient(cartItem.productName, cartItem.price);
        }

        Debug.Log($"[Inventory] Now holds {items.Count} item(s).");
        CartManager.Instance.Clear();
    }

    public void AddIngredient(string ingredientName, float pricePaid = 0f)
    {
        items.Add(new InventoryItem(ingredientName, pricePaid, InventoryItemKind.Ingredient));
    }

    public void AddCookedMeal(string mealName, string recipeName, MealQualityTier qualityTier)
    {
        InventoryItem cookedMeal = new InventoryItem(mealName, 0f, InventoryItemKind.CookedMeal)
        {
            mealName = mealName,
            sourceRecipeName = recipeName,
            qualityTier = qualityTier
        };

        items.Add(cookedMeal);
        Debug.Log($"[Inventory] Stored cooked meal '{mealName}' ({qualityTier}).");
    }

    public int CountByKind(InventoryItemKind kind)
    {
        int count = 0;
        foreach (InventoryItem item in items)
        {
            if (item.itemKind == kind)
                count++;
        }

        return count;
    }

    public bool HasAnyIngredients()
    {
        return CountByKind(InventoryItemKind.Ingredient) > 0;
    }

    public List<InventoryItem> RemoveAllIngredients()
    {
        List<InventoryItem> removedIngredients = new List<InventoryItem>();

        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].itemKind != InventoryItemKind.Ingredient)
                continue;

            removedIngredients.Add(items[i]);
            items.RemoveAt(i);
        }

        removedIngredients.Reverse();
        return removedIngredients;
    }

    public void LogInventory()
    {
        foreach (var item in items)
            Debug.Log($"[Inventory] {item.productName} ({item.itemKind}) — paid ${item.pricePaid:F2}");
    }

    // Add this method to InventoryManager
public void EatMeal(InventoryItem meal)
{
    if (meal == null || meal.itemKind != InventoryItemKind.CookedMeal) return;

    float moodBonus = meal.qualityTier switch
    {
        MealQualityTier.Simple    => 5f,
        MealQualityTier.Tasty     => 10f,
        MealQualityTier.Excellent => 15f,
        _                         => 5f
    };

    HungerManager.Instance?.Eat();
    MoodManager.Instance?.AdjustMood(moodBonus, $"{meal.mealName} was delicious!");
    items.Remove(meal);

    Debug.Log($"[Inventory] Ate '{meal.mealName}' ({meal.qualityTier}). Mood +{moodBonus}.");
}
}
