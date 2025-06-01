using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsPanelController : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsPanel;

    [Header("Audio")]
    public AudioSource globalAudioSource; 

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ShowSettingsPanel()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ExitToMainMenu()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadMainMenu();
    }

    public void AdjustVolume(float value)
    {
        if (globalAudioSource != null)
            globalAudioSource.volume = value;
    }
}
