using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsPanelController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas settingsPanelCanvas;
    [SerializeField] private Slider volumeSlider;
    private const string SETTINGS_CANVAS_TAG = "SettingsCanvas";


    [Header("Audio")]
    public AudioSource globalAudioSource;

    private void Start()
    {
        EnsureCanvasReference();
        InitializeSettingsPanel();
        InitializeVolumeSlider();
    }

    private void InitializeVolumeSlider()
    {
        if (volumeSlider != null && globalAudioSource != null)
        {
            volumeSlider.value = globalAudioSource.volume;
            volumeSlider.onValueChanged.AddListener(AdjustVolume);
        }
    }

    private void EnsureCanvasReference()
    {
        if (settingsPanelCanvas == null)
        {
            FindSettingsCanvas();
        }
    }

    private void FindSettingsCanvas()
    {
        GameObject canvasObj = GameObject.FindWithTag(SETTINGS_CANVAS_TAG);
        
        if (canvasObj != null)
        {
            settingsPanelCanvas = canvasObj.GetComponent<Canvas>();
        }
        else
        {
            canvasObj = GameObject.Find("SettingsUI");
            if (canvasObj != null)
            {
                settingsPanelCanvas = canvasObj.GetComponent<Canvas>();
            }
        }
    }

    private void InitializeSettingsPanel()
    {
        if (settingsPanelCanvas != null)
        {
            settingsPanelCanvas.gameObject.SetActive(false);
        }
    }

    public void ShowSettingsPanel()
    {
        if (settingsPanelCanvas == null || settingsPanelCanvas.gameObject == null)
        {
            FindSettingsCanvas();
            if (settingsPanelCanvas == null) return;
        }

        settingsPanelCanvas.gameObject.SetActive(true);
        Time.timeScale = 1f;
    }

    public void ResumeGame()
    {
        if (settingsPanelCanvas != null)
        {
            settingsPanelCanvas.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void ExitToMainMenu()
    {
        if (settingsPanelCanvas != null)
        {
            settingsPanelCanvas.gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
        SceneLoader.Instance?.LoadMainMenu();
    }

    public void AdjustVolume(float value)
    {
        if (globalAudioSource != null)
        {
            globalAudioSource.volume = Mathf.Clamp01(value);
        }
    }
}
