using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public enum IngredientType
{
    Solid,
    Liquid
}

[System.Serializable]
public struct Ingredient
{
    public string name;
    public IngredientType type;
}

[System.Serializable]
public struct Recipe
{
    public string name;
    public IngredientRequirement[] ingredients;
}

[System.Serializable]
public struct IngredientRequirement
{
    public Ingredient ingredient;
    public int amount;
}

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    [SerializeField] public Ingredient[] availableIngredients;
    [SerializeField] private Recipe[] recipes;
    
    private Dictionary<string, Recipe> recipeDictionary = new Dictionary<string, Recipe>();
    private List<Ingredient> currentMix = new List<Ingredient>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeRecipes();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Ejemplo de c�mo seleccionar ingredientes
    public Recipe CreateCustomRecipe(IngredientRequirement[] chosenIngredients)
    {
        return new Recipe
        {
            name = "Receta Personalizada",
            ingredients = chosenIngredients
        };
    }

    public void InitializeRecipes()
    {
        recipeDictionary.Clear();
        foreach(Recipe recipe in recipes)
        {
            if(!recipeDictionary.ContainsKey(recipe.name))
            {
                recipeDictionary.Add(recipe.name, recipe);
            }
        }
    }

    public Recipe GetRandomRecipe()
    {
        if (recipeDictionary.Count == 0) return default;
        
        List<Recipe> recipes = new List<Recipe>(recipeDictionary.Values);
        return recipes[UnityEngine.Random.Range(0, recipes.Count)];
    }

    public bool ValidateMix(List<Ingredient> mixedIngredients, out Recipe matchedRecipe)
    {
        foreach (var recipe in recipeDictionary.Values)
        {
            if (CheckRecipeMatch(recipe, mixedIngredients))
            {
                matchedRecipe = recipe;
                return true;
            }
        }
        matchedRecipe = default;
        return false;
    }

    private bool CheckRecipeMatch(Recipe recipe, List<Ingredient> ingredients)
    {
        if (recipe.ingredients.Length != ingredients.Count) return false;
        
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            if (!CompareIngredient(recipe.ingredients[i].ingredient, ingredients[i]))
            {
                return false;
            }
        }
        return true;
    }

    private bool CompareIngredient(Ingredient a, Ingredient b)
    {
        return a.name == b.name && a.type == b.type;
    }

    
}

