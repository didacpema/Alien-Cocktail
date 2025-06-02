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
        LogCurrentPanelStatus();
    }

    private void InitializeVolumeSlider()
    {
        if (volumeSlider != null && globalAudioSource != null)
        {
            volumeSlider.value = globalAudioSource.volume;
            volumeSlider.onValueChanged.AddListener(AdjustVolume);
        }
        else
        {
            Debug.LogError("Slider o AudioSource no asignados en el Inspector", this);
        }
    }

    private void EnsureCanvasReference()
    {
        // Si no está asignado en el inspector, intentar encontrarlo
        if (settingsPanelCanvas == null)
        {
            FindSettingsCanvas();
        }

        // Verificación final
        if (settingsPanelCanvas == null)
        {
            Debug.LogError("CRÍTICO: No se pudo encontrar el SettingsCanvas. Verifica:", this);
            Debug.LogError("1. ¿Existe un objeto con tag 'SettingsCanvas' en la escena?", this);
            Debug.LogError("2. ¿Tiene el componente Canvas?", this);
            Debug.LogError("3. ¿Está en la misma escena que este objeto?", this);
            
            // Opcional: Crear un canvas de emergencia
            // CreateEmergencyCanvas();
        }
    }

     private void FindSettingsCanvas()
    {
        GameObject canvasObj = GameObject.FindWithTag(SETTINGS_CANVAS_TAG);
        
        if (canvasObj != null)
        {
            settingsPanelCanvas = canvasObj.GetComponent<Canvas>();
            if (settingsPanelCanvas == null)
            {
                Debug.LogError($"El objeto con tag {SETTINGS_CANVAS_TAG} no tiene componente Canvas", canvasObj);
            }
            else
            {
                Debug.Log($"Canvas encontrado por tag: {canvasObj.name}", this);
            }
        }
        else
        {
            // Fallback: buscar por nombre
            canvasObj = GameObject.Find("SettingsUI");
            if (canvasObj != null)
            {
                settingsPanelCanvas = canvasObj.GetComponent<Canvas>();
                Debug.LogWarning($"Canvas encontrado por nombre. Asigna el tag '{SETTINGS_CANVAS_TAG}' para mejor rendimiento", canvasObj);
            }
        }
    }

    private void InitializeSettingsPanel()
    {
        if (settingsPanelCanvas != null)
        {
            settingsPanelCanvas.gameObject.SetActive(false);
            Debug.Log($"Panel inicializado. Estado inicial: oculto", settingsPanelCanvas.gameObject);
        }
    }

    private void LogCurrentPanelStatus()
    {
        if (settingsPanelCanvas != null)
        {
            Debug.Log($"Panel listo. Referencia: {settingsPanelCanvas.name}", settingsPanelCanvas.gameObject);
        }
    }
    
    public void ShowSettingsPanel()
    {
        
        Debug.Log("Evento ShowSettingsPanel recibido", this);
        
        // Verificación redundante por si la referencia se perdió
        if (settingsPanelCanvas == null || settingsPanelCanvas.gameObject == null)
        {
            Debug.LogWarning("La referencia al Canvas se perdió. Volviendo a buscar...", this);
            FindSettingsCanvas();
            
            if (settingsPanelCanvas == null)
            {
                Debug.LogError("No se puede mostrar el panel - Canvas no disponible", this);
                return;
            }
        }

        settingsPanelCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log($"Panel mostrado correctamente. Estado actual: {settingsPanelCanvas.gameObject.activeSelf}", settingsPanelCanvas.gameObject);
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
            Debug.Log($"Volumen ajustado a: {globalAudioSource.volume}");
        }
    }
}
