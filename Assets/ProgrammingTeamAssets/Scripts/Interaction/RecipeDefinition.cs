using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeDefinition", menuName = "Home Cooking/Recipe Definition")]
public class RecipeDefinition : ScriptableObject
{
    [Serializable]
    public class IngredientRequirement
    {
        public string ingredientName;
        public int quantity = 1;
    }

    public string recipeName;
    public string resultMealName;
    public List<IngredientRequirement> requiredIngredients = new List<IngredientRequirement>();

    [Tooltip("If true, the player knows this recipe from the start without needing to find it.")]
    public bool learnedByDefault = false;
}