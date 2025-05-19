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
    public float timeBetweenClients = 5f;
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
    public string CurrentRecipeName => currentClientRecipe.Equals(default(Recipe)) ? "Unknown" : currentClientRecipe.name;

    private float currentRemainingTime;
    private bool isOrderCompleted = false;
    private bool isWaitingForOrder = false;

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
        clientsSpawnedInShift++;
        currentAlien = Instantiate(alienPrefab, spawnPoint.position, spawnPoint.rotation);
        isOrderCompleted = false;

        yield return MoveToPosition(destinationPoint.position);

        currentClientRecipe = RecipeManager.Instance.GetRandomRecipe();

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

        while (currentRemainingTime > 0 && !isOrderCompleted)
        {
            currentRemainingTime -= Time.deltaTime;
            UpdateTimerText(Mathf.CeilToInt(currentRemainingTime));
            yield return null;
        }

        bool success = isOrderCompleted;
        float finalTime = isOrderCompleted ? currentRemainingTime : 0f;

        GameManager.Instance.CompleteOrder(success, finalTime, CurrentRecipeName);

        yield return ClientExitSequence();
    }

    private IEnumerator ClientExitSequence()
    {
        HideTimer();
        RecipeDisplay.Instance?.HideRecipe();

        yield return MoveToPosition(spawnPoint.position);

        DestroyCurrentClient();

        if (ShouldEndShift())
        {
            GameManager.Instance.EndShift();
        }
        else
        {
            yield return new WaitForSeconds(timeBetweenClients);
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

    private void DestroyCurrentClient()
    {
        if (currentAlien != null)
        {
            Destroy(currentAlien);
            currentAlien = null;
        }
    }

    private bool ShouldEndShift()
    {
        return clientsSpawnedInShift >= GameManager.Instance.maxClientsPerShift;
    }

    public void CompleteCurrentOrder()
    {
        if (isWaitingForOrder && !isOrderCompleted)
        {
            isOrderCompleted = true;
            isWaitingForOrder = false;
            if (timerText != null)
            {
                timerText.color = Color.blue;
            }
            if (RecipeDisplay.Instance != null)
            {
                RecipeDisplay.Instance.HideRecipe();
            }
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
            timerText.color = totalSeconds <= 15 ? Color.red : Color.green;
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
}