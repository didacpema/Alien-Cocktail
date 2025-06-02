using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("VR References")]
    public XROrigin vrOrigin; 
    public XRInteractionManager interactionManager;


    [Header("Game Settings")]
    public int maxClientsPerShift = 2; //cambiar a 5 más adelante


    [Header("Managers")]
    public RecipeManager recipeManager;
    public ClientManager clientManager; 
    public ResultManager resultManager; 

    [Header("Time Settings")]
    public float totalOrderTime = 60f;

    private int currentScore;
    private int clientsServed;
    private bool isShiftActive;

    public enum OrderGrade { Excellent, Good, Failed }
    public OrderGrade lastOrderGrade { get; set; }

    [Header("Puntuation")]
    public int excellentPoints = 100;
    public int goodPoints = 60;
    public int failedPoints = 0;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene") InitializeGame();
    }

    private void InitializeGame()
    {
        InitializeVR();
        StartNewShift();
    }

    private void InitializeVR()
    {
        if (vrOrigin == null)
            vrOrigin = FindObjectOfType<XROrigin>(); 

        if (interactionManager == null)
            interactionManager = FindObjectOfType<XRInteractionManager>();
    }

    public void StartNewShift()
    {
        currentScore = 0;
        clientsServed = 0;
        isShiftActive = true;

        recipeManager.InitializeRecipes();
        clientManager.StartClientCycle(maxClientsPerShift);
    }

    public void CompleteOrder(bool success, float timeRemaining, string recipeName = null)
    {
        if (!isShiftActive) return;

        // Si no se proporciona recipeName, intentar obtenerlo del cliente actual
        if (string.IsNullOrEmpty(recipeName))
        {
            recipeName = "Receta Desconocida";
            if (clientManager != null && !clientManager.CurrentClientRecipe.Equals(default(Recipe)))
            {
                recipeName = clientManager.CurrentClientRecipe.name;
            }
        }

        lastOrderGrade = CalculateGrade(success, timeRemaining);
        int pointsEarned = GetPointsFromGrade(lastOrderGrade);
        float timeUsed = totalOrderTime - timeRemaining;

        currentScore += pointsEarned;
        clientsServed++;

        // Guardar el resultado en el ResultManager
        resultManager.SaveRecipeResult(recipeName, lastOrderGrade, pointsEarned, timeUsed, success);

        Debug.Log($"Orden completada: {recipeName} - {lastOrderGrade} - {pointsEarned} puntos");

        if (clientsServed >= maxClientsPerShift)
        {
            EndShift();
        }
    }

    private OrderGrade CalculateGrade(bool success, float timeRemaining)
    {
        if (!success) return OrderGrade.Failed;

        if (timeRemaining > 100f) return OrderGrade.Excellent;
        if (timeRemaining > 15f) return OrderGrade.Good;
        return OrderGrade.Failed;
    }

    private int GetPointsFromGrade(OrderGrade grade)
    {
        return grade switch
        {
            OrderGrade.Excellent => excellentPoints,    // 100 puntos
            OrderGrade.Good => goodPoints,              // 60 puntos
            OrderGrade.Failed => failedPoints,          // 0 puntos
            _ => 0
        };
    }

    public void EndShift()
    {
        isShiftActive = false;
        resultManager.CalculateFinalResults(currentScore); 
        SceneLoader.Instance.LoadResultsScene();
    }

    public void RestartGame()
    {
        currentScore = 0;
        clientsServed = 0;
        isShiftActive = false;

        resultManager.ClearResults();
        clientManager.ClearClients();

        StartNewShift();
    }

    public int GetCurrentScore() => currentScore;
}