using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class ClientManager : MonoBehaviour
{
    [Header("Client Settings")]
    public GameObject alienPrefab;
    public float moveSpeed = 1.5f;
    public float waitingTime = 90f;

    [Header("References")]
    private Transform spawnPoint;
    private Transform destinationPoint;
    private GameObject currentAlien;
    private bool pointsInitialized = false;
    private GameObject timerUIInstance;
    private TMP_Text timerText;
    private int clientsSpawnedInShift = 0;

    [Header("UI Settings")]
    public GameObject timerTextPrefab;
    public float timerHeightOffset = 0.5f;

    [Header("Recipe Settings")]
    private Recipe currentClientRecipe;
    public Recipe CurrentClientRecipe => currentClientRecipe;

    private float currentRemainingTime;
    private bool isOrderCompleted = false;
    private bool isWaitingForOrder = false;

    private Machine machine;
    public static ClientManager Instance { get; private set; }

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
        if (scene.name == "GameScene") InitializeSpawnPoints();
    }
    private void InitializeSpawnPoints()
    {
        pointsInitialized = false;
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint")?.transform;
        destinationPoint = GameObject.FindGameObjectWithTag("Destination")?.transform;
        pointsInitialized = spawnPoint != null && destinationPoint != null;

        if (!pointsInitialized)
            Debug.LogError("Failed to initialize spawn points");
    }

    public void StartClientCycle(int maxClients)
    {
        clientsSpawnedInShift = 0;
        Debug.Log($"Preparando ciclo de clientes. Máximo: {maxClients}");
        InitializeSpawnPoints();
        GenerateNewClient();
    }

    public void GenerateNewClient()
    {
        if (!pointsInitialized)
        {
            StartCoroutine(RetryGenerateClient());
            return;
        }

        if (currentAlien == null && clientsSpawnedInShift < GameManager.Instance.maxClientsPerShift)
        {
            StartCoroutine(MoveClient());
        }
    }

    private IEnumerator RetryGenerateClient()
    {
        yield return new WaitForEndOfFrame();
        GenerateNewClient();
    }

    private IEnumerator MoveClient()    
    {
        if (clientsSpawnedInShift >= GameManager.Instance.maxClientsPerShift)
        {
            yield break;
        }

        clientsSpawnedInShift++;
        Debug.Log($"Generando cliente {clientsSpawnedInShift}/{GameManager.Instance.maxClientsPerShift}");
        currentAlien = Instantiate(alienPrefab, spawnPoint.position, spawnPoint.rotation);
        isOrderCompleted = false;

        yield return MoveToPosition(destinationPoint.position);

        currentClientRecipe = RecipeManager.Instance.GetRandomRecipe();
        Machine.Instance.NewOrder(currentClientRecipe);

        if (currentClientRecipe.Equals(default(Recipe)))
        {
            Debug.LogError("No hay recetas disponibles");
        }

        if (RecipeDisplay.Instance != null)
        {
            RecipeDisplay.Instance.ShowRecipe(currentClientRecipe);
        }

        ShowTimer();
        currentRemainingTime = waitingTime;
        isWaitingForOrder = true;

        while (currentRemainingTime > 0 && !isOrderCompleted && isWaitingForOrder)
        {
            currentRemainingTime -= Time.deltaTime;
            UpdateTimerText(Mathf.CeilToInt(currentRemainingTime));
            yield return null;
        }

        if (isOrderCompleted)
        {
            if (timerText != null) timerText.color = Color.green;
            yield return new WaitForSeconds(2f); 
        }

        HideTimer();
        if (RecipeDisplay.Instance != null)
        {
            RecipeDisplay.Instance.HideRecipe();
        }
        isWaitingForOrder = false;

        yield return MoveToPosition(spawnPoint.position);
        
        GameManager.Instance.CompleteOrder(isOrderCompleted, currentRemainingTime);

        Destroy(currentAlien);
        currentAlien = null;

        if (clientsSpawnedInShift < GameManager.Instance.maxClientsPerShift)
        {
            yield return new WaitForSeconds(1f); 
            GenerateNewClient();
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        if (currentAlien == null) yield break;

        while (Vector3.Distance(currentAlien.transform.position, targetPosition) > 0.1f)
        {
            currentAlien.transform.position = Vector3.MoveTowards(
                currentAlien.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private void ShowTimer()
    {
        if (timerTextPrefab == null) return;

        timerUIInstance = Instantiate(timerTextPrefab);
        timerText = timerUIInstance.GetComponentInChildren<TMP_Text>();

        if (currentAlien != null)
        {
            timerUIInstance.transform.position = currentAlien.transform.position + Vector3.up * timerHeightOffset;
            timerUIInstance.transform.SetParent(currentAlien.transform);
        }
    }

    private void UpdateTimerText(int totalSeconds)
    {
        if (timerText != null)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            timerText.text = $"{minutes:00}:{seconds:00}";
            timerText.color = totalSeconds <= 15 ? Color.red : Color.white;
        }
    }

    private void HideTimer()
    {
        if (timerUIInstance != null)
        {
            Destroy(timerUIInstance);
        }
    }

    public float GetCurrentRemainingTime()
    {
        return isWaitingForOrder ? currentRemainingTime : 0f;
    }
    
    public void ClearClients()
    {
        clientsSpawnedInShift = 0;
        isOrderCompleted = false;
        isWaitingForOrder = true;

    }

    public void CompleteOrderWithDrink()
    {
        if (isWaitingForOrder && !isOrderCompleted)
        {
            isOrderCompleted = true;
        }
    }
}