using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;

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
    public bool isDrinking = false;
    private int currentDone = 0;
    public GameObject DrinkPrefab;
    public Transform drinkTransform;
    public Animator animator;
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
        // if (!isDone && currentRequirements.Count != 0)
        // {
        //     for (int i = 0; i < currentRequirements.Count; i++)
        //     {
        //         if(currentRequirements[i].ingredient.Completed && currentRequirements[i].ingredient.Done == false)
        //         {
        //             var ing = currentRequirements[i];
        //             ing.ingredient.Done = true;
        //             currentRequirements[i] = ing;
        //             currentDone++;
        //         }
        //     }
        //     if (currentDone == 5)
        //     {
        //         isDone = true;
        //         currentDone = 0; // Reset currentDone after checking
        //         Debug.Log("Order completed!");
        //     }
        // }

    }
    private void UpdateOrder()
    {
        if (!isDone )
        {
            for (int i = 0; i < currentRequirements.Count; i++)
            {
                if(currentRequirements[i].ingredient.Completed && currentRequirements[i].ingredient.Done == false)
                {
                    var ing = currentRequirements[i];
                    ing.ingredient.Done = true;
                    currentRequirements[i] = ing;
                    currentDone++;
                }
            }
            if (currentDone == 5)
            {
                isDone = true;
                currentDone = 0; // Reset currentDone after checking
                SpawnDrink();
            }
        }
    }
    public void NewOrder(Recipe recipe)
    {
        Text.text = "";
        currentRecipe = recipe;
        for (int i = 0; i < currentRequirements.Count; i++)
        {
            var ing = currentRequirements[i];
            ing.ingredient.Done = false;
            ing.ingredient.Completed = false;
            currentRequirements[i] = ing;
        }
        currentRequirements.Clear();
        currentDone = 0;
        isDone = false;
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
            else if (ingredientRequirement.ingredient.type == IngredientType.Solid)
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
            bool ingredientFound = false;
            
            for (int i = 0; i < currentRequirements.Count; i++)
            {
                if (currentRequirements[i].ingredient.type == IngredientType.Liquid && currentRequirements[i].ingredient.name == tag)
                {
                    ingredientFound = true;
                    var req = currentRequirements[i];
                    if (req.amount > 0f) { req.amount -= Time.deltaTime * 4; }
                    else { req.amount = 0; }

                    currentRequirements[i] = req;
                    UpdateText();
                    
                    if (currentRequirements[i].amount <= 0f)
                    {
                        var ing = currentRequirements[i];
                        ing.ingredient.Completed = true;
                        currentRequirements[i] = ing;
                        UpdateOrder();
                        break;
                    }
                }
                else if (currentRequirements[i].ingredient.type == IngredientType.Solid && currentRequirements[i].ingredient.name == tag)
                {
                    ingredientFound = true;
                    animator.SetTrigger("eat"); 
                    StartCoroutine(ResetToIdle(0.66f)); 
                    
                    var req = currentRequirements[i];
                    if (req.amount > 0f) { req.amount -= 1; }
                    else { req.amount = 0; }
                    currentRequirements[i] = req;
                    UpdateText();
                    
                    if (currentRequirements[i].amount <= 0f)
                    {
                        var ing = currentRequirements[i];
                        ing.ingredient.Completed = true;
                        currentRequirements[i] = ing;
                        UpdateOrder();
                        break;
                    }
                }
            }

            if (!ingredientFound)
            {
                animator.SetTrigger("no");
                StartCoroutine(ResetToIdle(2.7f)); 
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
            else if (req.ingredient.type == IngredientType.Solid)
            {
                Text.text += $"{req.ingredient.name} x {req.amount}\n";
            }
        }
    }

    private void SpawnDrink()
    {
        animator.SetTrigger("prepare");
        StartCoroutine(ResetToIdle(7.33f)); 
        GameObject drink = Instantiate(DrinkPrefab, drinkTransform.position, drinkTransform.rotation);
    }
    
    private IEnumerator ResetToIdle(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.ResetTrigger("no");
        animator.ResetTrigger("prepare");
        animator.ResetTrigger("eat");
        animator.Play("Idle"); 
    }
}
