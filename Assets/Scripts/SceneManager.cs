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

    public void LoadDemoScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoScene");

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
        LoadDemoScene();
    }

    public void OnShiftCompleted()
    {
        LoadResultsScene();
    }
}