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
    public GameObject GoodDrinkPrefab;
    public GameObject ExcellentDrinkPrefab;
    public GameObject BadDrinkPrefab;
    int drinkRating = 0; // 0: Excellent, 1-3: Good, 3-5: Bad
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
            drinkRating = 0; // Reset drink rating for the next order
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
            for (int i = 0; i < currentRequirements.Count; i++)
            {
                if (currentRequirements[i].ingredient.type == IngredientType.Liquid && currentRequirements[i].ingredient.name == tag)
                {
                    var req = currentRequirements[i];
                    req.amount -= Time.deltaTime * 4;
                    UpdateText();
                    currentRequirements[i] = req;
                    // Check if the fill amount exceeds the required amount
                    if (currentRequirements[i].amount <= 0f)
                    {
                        var ing = currentRequirements[i];
                        ing.ingredient.Completed = true;
                        currentRequirements[i] = ing;
                        if( currentRequirements[i].amount < 0f && currentRequirements[i].amount > -5f){drinkRating++;}
                        else if (currentRequirements[i].amount <= -5f){drinkRating += 2;}
                        break;
                    }
                }
                else if (currentRequirements[i].ingredient.type == IngredientType.Solid && currentRequirements[i].ingredient.name == tag)
                {
                    var req = currentRequirements[i];
                    req.amount -= 1f;
                    UpdateText();
                    currentRequirements[i] = req;
                    // Check if the fill amount exceeds the required amount
                    if (currentRequirements[i].amount <= 0f)
                    {
                        var ing = currentRequirements[i];
                        ing.ingredient.Completed = true;
                        currentRequirements[i] = ing;
                        if( currentRequirements[i].amount < 0f && currentRequirements[i].amount > -5f){drinkRating++;}
                        else if (currentRequirements[i].amount <= -5f){drinkRating += 2;}

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
        switch (drinkRating)
        {
            case 0: // Excellent
                Instantiate(ExcellentDrinkPrefab, drinkTransform.position, Quaternion.identity);
                Debug.Log("Excellent drink spawned.");
                break;
            case 1 or 2 or 3 or 4 or 5: // Good
                Instantiate(GoodDrinkPrefab, drinkTransform.position, Quaternion.identity);
                Debug.Log("Good drink spawned.");
                break;
            case > 5: // Bad
                Instantiate(BadDrinkPrefab, drinkTransform.position, Quaternion.identity);
                Debug.Log("Bad drink spawned.");
                break;
            default:
                Debug.LogWarning("Invalid drink rating, using default prefab.");
                Instantiate(BadDrinkPrefab, drinkTransform.position, Quaternion.identity);
                break;
        }

    }
}
