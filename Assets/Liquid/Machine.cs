using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Machine : MonoBehaviour
{
    public static Machine Instance { get; private set; }
    private List<IngredientRequirement> currentRequirements = new List<IngredientRequirement>();

    public TextMeshProUGUI Text;
    public RecipeManager recipeManager;
    public ClientManager clientManager;
    private CompleteOrderTrigger completeOrderTrigger;
    private Recipe currentRecipe;
    public bool isDone = false;
    public GameObject DrinkPrefab;
    public Transform drinkTransform;
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
        recipeManager = FindAnyObjectByType<RecipeManager>();
        clientManager = FindAnyObjectByType<ClientManager>();
        completeOrderTrigger = FindAnyObjectByType<CompleteOrderTrigger>();
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

        if (isDone)
        {
            SpawnDrink();
            isDone = false; // Reset isDone after spawning the drink
        }

    }
    public void NewOrder(Recipe recipe)
    {
        Text.text = "";
        currentRecipe = recipe;
        currentRequirements.Clear();
        foreach (var ingredientRequirement in currentRecipe.ingredients)
        {
            currentRequirements.Add(new IngredientRequirement
            {
                ingredient = ingredientRequirement.ingredient,
                amount = ingredientRequirement.amount
            });
            if (ingredientRequirement.ingredient.type == IngredientType.Liquid)
            {
                Text.text += $"{ingredientRequirement.ingredient.name}  {ingredientRequirement.amount}ml\n";
            }
            else
            {
                Text.text += $"{ingredientRequirement.ingredient.name} x {ingredientRequirement.amount}\n";
            }

        }
    }

    public void Fill(string tag)
    {
        if (currentRequirements.Count == 0)
        {
            Debug.LogWarning("No current requirements to fill.");
        }
        else
        {
            for (int i = 0; i < currentRequirements.Count; i++)
            {
                if (currentRequirements[i].ingredient.type == IngredientType.Liquid && currentRequirements[i].ingredient.name == tag)
                {
                    var req = currentRequirements[i];
                    if (req.amount > 0f) { req.amount -= Time.deltaTime * 4; }
                    else {req.amount = 0; }

                    UpdateText();
                    currentRequirements[i] = req;
                    // Check if the fill amount exceeds the required amount
                    if (currentRequirements[i].amount <= 0f)
                    {
                        var ing = currentRequirements[i];
                        ing.ingredient.Completed = true;
                        currentRequirements[i] = ing;
                        break;
                    }
                }
                else if (currentRequirements[i].ingredient.type == IngredientType.Solid && currentRequirements[i].ingredient.name == tag)
                {
                    var req = currentRequirements[i];
                    if (req.amount > 0f) { req.amount -= 1; }
                    else {req.amount = 0; }
                    UpdateText();
                    currentRequirements[i] = req;
                    // Check if the fill amount exceeds the required amount
                    if (currentRequirements[i].amount <= 0f)
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
    void UpdateText()
    {
        Text.text = ""; // Clear the text before updating
        foreach (var req in currentRequirements)
        {
            if (req.ingredient.type == IngredientType.Liquid)
            {
                Text.text += $"{req.ingredient.name}  {req.amount}ml\n";
            }
            else
            {
                Text.text += $"{req.ingredient.name} x {req.amount}\n";
            }
        }
    }

    private void SpawnDrink()
    {
        completeOrderTrigger.Drink(currentRecipe.name);
        switch (currentRecipe.name)
        {
            case "Allmighty": // Excellent
                Instantiate(DrinkPrefab, drinkTransform.position, Quaternion.identity);
                DrinkPrefab.gameObject.tag = "Allmighty";
                Debug.Log("Allmighty drink spawned.");
                break;
            case "Nebula Nectar": // Good
                Instantiate(DrinkPrefab, drinkTransform.position, Quaternion.identity);
                DrinkPrefab.gameObject.tag = "Nebula Nectar";
                Debug.Log("Nebula Nectar drink spawned.");
                break;
            case "Cosmic Kiss": // Bad
                Instantiate(DrinkPrefab, drinkTransform.position, Quaternion.identity);
                DrinkPrefab.gameObject.tag = "Cosmic Kiss";
                Debug.Log("Cosmic Kiss drink spawned.");
                break;
            case "ZeroGravity": // Bad
                Instantiate(DrinkPrefab, drinkTransform.position, Quaternion.identity);
                DrinkPrefab.gameObject.tag = "ZeroGravity";
                Debug.Log("ZeroGravity drink spawned.");
                break;
            case "Stellar Seduction": // Bad
                Instantiate(DrinkPrefab, drinkTransform.position, Quaternion.identity);
                DrinkPrefab.gameObject.tag = "Stellar Seduction";
                Debug.Log("Stellar Seduction drink spawned.");
                break;
            case "Sweet Lele": // Bad
                Instantiate(DrinkPrefab, drinkTransform.position, Quaternion.identity);
                DrinkPrefab.gameObject.tag = "Sweet Lele";
                Debug.Log("Sweet Lele drink spawned.");
                break;
            default:
                Debug.LogWarning($"Unknown recipe: {currentRecipe.name}. No drink spawned.");
                return;
        }

    }
}
