using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("VR References")]
    public XROrigin vrOrigin; 
    public XRInteractionManager interactionManager;


    [Header("Game Settings")]
    public float baseTimePerOrder = 90f;
    public int maxClientsPerShift = 5;


    [Header("Managers")]
    public RecipeManager recipeManager;
    public ClientManager clientManager; 
    public ResultManager resultManager; 

    private int currentScore;
    private int clientsServed;
    private bool isShiftActive;

    public enum OrderGrade { Excellent, Notable, Good, Sufficient, Failed }
    private OrderGrade lastOrderGrade;

    [Header("Puntuaci�n")]
    public int excellentPoints = 100;
    public int notablePoints = 80;
    public int goodPoints = 60;
    public int sufficientPoints = 40;
    public int failedPoints = 0;

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

    private void Start()
    {
        InitializeVR();
        Debug.Log("Iniciando nuevo turno...");
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
        clientManager.GenerateNewClient(); 
    }


    //el gameManager calcula la puntuaci�n y el resultManager procesa y muestra los resultados finales!!
    //esta funcion deberia llamarse en el clientManager!!
    public void CompleteOrder(bool success, float timeRemaining)
    {
        if (!isShiftActive) return;

        lastOrderGrade = CalculateGrade(success, timeRemaining);
        int pointsEarned = GetPointsFromGrade(lastOrderGrade);

        currentScore += pointsEarned;
        clientsServed++;

        if (clientsServed >= maxClientsPerShift)
        {
            EndShift();
        }
        else
        {
            clientManager.GenerateNewClient(); 
        }
    }
    private OrderGrade CalculateGrade(bool success, float timeRemaining)
    {
        if (!success) return OrderGrade.Failed;

        if (timeRemaining > 45f) return OrderGrade.Excellent;
        if (timeRemaining > 30f) return OrderGrade.Notable;
        if (timeRemaining > 15f) return OrderGrade.Good;
        if (timeRemaining > 1f) return OrderGrade.Sufficient;
        return OrderGrade.Failed;
    }

    private int GetPointsFromGrade(OrderGrade grade)
    {
        return grade switch
        {
            OrderGrade.Excellent => excellentPoints,
            OrderGrade.Notable => notablePoints,
            OrderGrade.Good => goodPoints,
            OrderGrade.Sufficient => sufficientPoints,
            _ => failedPoints
        };
    }

    private void EndShift()
    {
        isShiftActive = false;
        resultManager.CalculateFinalResults(currentScore); 
        SceneLoader.Instance.LoadResultsScene();
    }

    public int GetCurrentScore() => currentScore;
}