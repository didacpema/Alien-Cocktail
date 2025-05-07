using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Management;

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
    public SceneManager sceneManager;

    private int currentScore;
    private int clientsServed;
    private bool isShiftActive;

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

        //recipeManager.InitializeRecipes(); 
        //clientManager.GenerateNewClient(); 
    }

    public void CompleteOrder(bool success, float timeRemaining)
    {
        if (!isShiftActive) return;

        int pointsEarned = success ?
            Mathf.RoundToInt(100 + (timeRemaining * 0.5f)) :
            -50;

        currentScore += pointsEarned;
        clientsServed++;

        if (clientsServed >= maxClientsPerShift)
        {
            EndShift();
        }
        else
        {
            //clientManager.GenerateNewClient(); 
        }
    }

    private void EndShift()
    {
        isShiftActive = false;
        //resultManager.CalculateFinalResults(currentScore); 
        //sceneManager.LoadResultsScene();
    }

    public int GetCurrentScore() => currentScore;
}