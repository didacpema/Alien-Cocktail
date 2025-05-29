using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Machine : MonoBehaviour
{
    public static Machine Instance { get; private set; }
    private List<IngredientRequirement> currentRequirements = new List<IngredientRequirement>();

    public float fillAmount = 0f;
    public TextMeshProUGUI Text;
    public RecipeManager recipeManager;
    public ClientManager clientManager;
    private Recipe currentRecipe;
    public bool isDone = false;
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
    void Start()
    {

    }

    void Update()
    {
        foreach (var req in currentRequirements)
        {
            if (req.amount <= 0f && !req.ingredient.Completed)
            {
                isDone = true; // Mark the machine as done when all requirements are met
                Debug.Log($"Machine is done with recipe: {currentRecipe.name}");
            }
        }
    }
    public void NewOrder(Recipe recipe)
    {
        currentRecipe = recipe;
        currentRequirements.Clear();
        foreach (var ingredientRequirement in currentRecipe.ingredients)
        {
            currentRequirements.Add(new IngredientRequirement
            {
                ingredient = ingredientRequirement.ingredient,
                amount = ingredientRequirement.amount
            });
            if(ingredientRequirement.ingredient.type == IngredientType.Liquid)
            {
                Text.text += $"{ingredientRequirement.ingredient.name}  {ingredientRequirement.amount}ml\n";
            }
            else
            {
                Text.text += $"{ingredientRequirement.ingredient.name} x {ingredientRequirement.amount}\n";
            }
            
        }
        fillAmount = 0f; // Reset fill amount for new order
    }
    public float GetMachineFillAmount()
    {
        return fillAmount;
    }

    public void Fill(string tag)
    {
        if (currentRequirements.Count == 0)
        {
            Debug.LogWarning("No current requirements to fill.");
        }
        else
        {
            for(int i = 0; i < currentRequirements.Count; i++)
            {
                if (currentRequirements[i].ingredient.name == tag)
                {
                    // If the ingredient is already in the requirements, fill it
                    var req = currentRequirements[i];
                    req.amount -= Time.deltaTime;
                    currentRequirements[i] = req;
                    // Check if the fill amount exceeds the required amount
                    if (currentRequirements[i].amount <= 0f )
                    {
                        var ing = currentRequirements[i];
                        ing.ingredient.Completed = true;
                        currentRequirements[i] = ing;
                        break; 
                    }
                    
                }
            }
        }
    }
}
