using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

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

    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewShift();
        }
    }

    public void LoadResultsScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ResultsScene");

        //if (GameManager.Instance != null && GameManager.Instance.resultManager != null)
        //{
        //    GameManager.Instance.resultManager.CalculateFinalResults(GameManager.Instance.GetCurrentScore());
        //}
    }

    public void OnReturnToMainMenu()
    {
        LoadMainMenu();
    }

    public void OnPlayAgain()
    {
        LoadGameScene();
    }

    public void OnShiftCompleted()
    {
        LoadResultsScene();
    }
}