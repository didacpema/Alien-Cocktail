using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Machine : MonoBehaviour
{
    private struct IngredientFill { }
    public float fillAmount = 0f;
    public TextMeshProUGUI fillAmountText; 
    public RecipeManager recipeManager;
    public ClientManager clientManager;
    private Recipe currentRecipe;

    void Start()
    {

    }

    void Update()
    {
        fillAmountText.text = "Fill Amount: " + fillAmount.ToString("F2"); // Update the UI text with the fill amount
    }
    public void NewOrder(Recipe recipe)
    {
        currentRecipe = recipe;        
        fillAmount = 0f; // Reset fill amount for new order
        fillAmountText.text = "Fill Amount: " + fillAmount.ToString("F2"); // Update the UI text with the reset fill amount
    }
    public float GetMachineFillAmount()
    {
        return fillAmount;
    }
    public float Fill(string tag)
    {
        

        fillAmount += Time.deltaTime;
        return fillAmount;
    }
}
