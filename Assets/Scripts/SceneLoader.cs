using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private const string STORY_SHOWN_KEY = "StoryShown";

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
        SceneManager.LoadScene("MainMenu");

    }

    public void LoadGameScene(bool forceStory = false)
    {
        if (!forceStory && PlayerPrefs.GetInt(STORY_SHOWN_KEY, 0) == 1)
        {
            LoadGameSceneDirectly();
        }
        else
        {
            LoadGameWithStory();
        }
    }

    public void LoadGameWithStory()
    {
        PlayerPrefs.SetInt(STORY_SHOWN_KEY, 1);
        SceneManager.LoadScene("StoryScene");
        GameManager.Instance?.StartNewShift();
        ResultManager.Instance?.ClearResults();
    }

    public void LoadGameSceneDirectly()
    {
        SceneManager.LoadScene("GameScene");
        GameManager.Instance?.StartNewShift();
    }

    public void LoadResultsScene()
    {
        SceneManager.LoadScene("ResultsScene");
    }
}