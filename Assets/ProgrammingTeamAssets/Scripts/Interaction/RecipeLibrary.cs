using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeLibrary", menuName = "Home Cooking/Recipe Library")]
public class RecipeLibrary : ScriptableObject
{
    public List<RecipeDefinition> recipes = new List<RecipeDefinition>();
}
