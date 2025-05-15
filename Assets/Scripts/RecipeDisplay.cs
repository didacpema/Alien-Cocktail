using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeDisplay : MonoBehaviour
{
    public static RecipeDisplay Instance { get; private set; }
    
    [SerializeField] public TMP_Text recipeText;
    [SerializeField] public Canvas vrCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowRecipe(Recipe recipe)
    {
        vrCanvas.enabled = true;
        string recipeString = $"Receta: {recipe.name}\n";
        
        foreach (var ingredient in recipe.ingredients)
        {
            recipeString += $"- {ingredient.ingredient.name} x{ingredient.amount}\n";
        }
        
        recipeText.text = recipeString;
    }

    public void HideRecipe()
    {
        vrCanvas.enabled = false;
    }
}
