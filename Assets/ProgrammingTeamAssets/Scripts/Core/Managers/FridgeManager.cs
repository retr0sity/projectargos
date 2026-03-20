using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Persistent ingredient stash used by the Home fridge.
/// Ingredients are stored as stacks and survive scene transitions.
/// </summary>
public class FridgeManager : MonoBehaviour
{
    [Serializable]
    public class IngredientStack
    {
        public string ingredientName;
        public int quantity;

        public IngredientStack(string ingredientName, int quantity)
        {
            this.ingredientName = ingredientName;
            this.quantity = quantity;
        }
    }

    [Serializable]
    public class PendingMealData
    {
        public string recipeName;
        public string resultMealName;

        public PendingMealData(string recipeName, string resultMealName)
        {
            this.recipeName = recipeName;
            this.resultMealName = resultMealName;
        }
    }

    private static FridgeManager instance;

    public static FridgeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FridgeManager>();
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("FridgeManager");
                    instance = managerObject.AddComponent<FridgeManager>();
                }
            }

            return instance;
        }
    }

    [SerializeField] private List<IngredientStack> storedIngredients = new List<IngredientStack>();
    [SerializeField] private PendingMealData pendingMeal;

    public IReadOnlyList<IngredientStack> StoredIngredients => storedIngredients;
    public PendingMealData PendingMeal => pendingMeal;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int StoreAllIngredientsFromInventory(InventoryManager inventoryManager)
    {
        if (inventoryManager == null)
            return 0;

        List<InventoryManager.InventoryItem> removedIngredients = inventoryManager.RemoveAllIngredients();
        return StoreIngredients(removedIngredients);
    }

    public int StoreIngredients(IEnumerable<InventoryManager.InventoryItem> ingredients)
    {
        int storedCount = 0;

        foreach (InventoryManager.InventoryItem ingredient in ingredients)
        {
            if (ingredient == null || ingredient.itemKind != InventoryItemKind.Ingredient)
                continue;

            AddIngredient(ingredient.productName, 1);
            storedCount++;
        }

        return storedCount;
    }

    public bool HasPendingMeal()
    {
        return pendingMeal != null;
    }

    public bool CanCook(RecipeDefinition recipe)
    {
        if (recipe == null)
            return false;

        foreach (RecipeDefinition.IngredientRequirement requirement in recipe.requiredIngredients)
        {
            if (GetStoredQuantity(requirement.ingredientName) < requirement.quantity)
                return false;
        }

        return true;
    }

    public List<string> GetMissingIngredients(RecipeDefinition recipe)
    {
        List<string> missingIngredients = new List<string>();

        if (recipe == null)
            return missingIngredients;

        foreach (RecipeDefinition.IngredientRequirement requirement in recipe.requiredIngredients)
        {
            int shortage = requirement.quantity - GetStoredQuantity(requirement.ingredientName);
            if (shortage > 0)
                missingIngredients.Add($"{requirement.ingredientName} x{shortage}");
        }

        return missingIngredients;
    }

    public bool TryStartRecipe(RecipeDefinition recipe)
    {
        if (recipe == null || HasPendingMeal() || !CanCook(recipe))
            return false;

        foreach (RecipeDefinition.IngredientRequirement requirement in recipe.requiredIngredients)
        {
            RemoveIngredient(requirement.ingredientName, requirement.quantity);
        }

        pendingMeal = new PendingMealData(recipe.recipeName, recipe.resultMealName);
        return true;
    }

    public void ClearPendingMeal()
    {
        pendingMeal = null;
    }

    public string BuildContentsSummary()
    {
        if (storedIngredients.Count == 0)
            return "Fridge is empty.";

        StringBuilder builder = new StringBuilder("Fridge Contents:");
        for (int i = 0; i < storedIngredients.Count; i++)
        {
            IngredientStack stack = storedIngredients[i];
            builder.AppendLine();
            builder.Append("- ");
            builder.Append(stack.ingredientName);
            builder.Append(" x");
            builder.Append(stack.quantity);
        }

        return builder.ToString();
    }

    private void AddIngredient(string ingredientName, int quantity)
    {
        if (string.IsNullOrWhiteSpace(ingredientName) || quantity <= 0)
            return;

        IngredientStack stack = storedIngredients.Find(item => item.ingredientName == ingredientName);
        if (stack == null)
        {
            storedIngredients.Add(new IngredientStack(ingredientName, quantity));
            return;
        }

        stack.quantity += quantity;
    }

    private void RemoveIngredient(string ingredientName, int quantity)
    {
        if (quantity <= 0)
            return;

        IngredientStack stack = storedIngredients.Find(item => item.ingredientName == ingredientName);
        if (stack == null)
            return;

        stack.quantity -= quantity;
        if (stack.quantity <= 0)
            storedIngredients.Remove(stack);
    }

    private int GetStoredQuantity(string ingredientName)
    {
        IngredientStack stack = storedIngredients.Find(item => item.ingredientName == ingredientName);
        return stack != null ? stack.quantity : 0;
    }
}
