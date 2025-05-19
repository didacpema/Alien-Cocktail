using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class StorySceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private float timePerPhrase = 3f;
    [SerializeField] private Canvas storyCanvas;

    [TextArea(3, 10)]
    [SerializeField] private string[] storyPhrases;

    private void Start()
    {
        StartCoroutine(InitializeVRCanvas());
    }

    private IEnumerator InitializeVRCanvas()
    {
        yield return new WaitUntil(() => XRSettings.isDeviceActive || XRSettings.loadedDeviceName != "None");

        ConfigureCanvasForVR();
        StartCoroutine(ShowStory());
    }

    private void ConfigureCanvasForVR()
    {
        if (XRSettings.isDeviceActive)
        {
            storyCanvas.renderMode = RenderMode.WorldSpace;
            storyCanvas.worldCamera = Camera.main;

            CanvasFaceCamera();
        }
        else
        {
            storyCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        storyText.text = "";
    }

    private void CanvasFaceCamera()
    {
        if (Camera.main != null)
        {
            storyCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    private IEnumerator ShowStory()
    {
        if (storyPhrases == null || storyPhrases.Length == 0)
        {
            Debug.LogError("No hay frases configuradas en StorySceneManager");
            yield break;
        }

        foreach (string phrase in storyPhrases)
        {
            if (storyText != null)
            {
                storyText.text = phrase;
                yield return new WaitForSeconds(timePerPhrase);
            }
            else
            {
                Debug.LogError("storyText no está asignado en el inspector");
                yield break;
            }
        }

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader.Instance es null!");
        }
        else
        {
            Debug.Log("Cargando GameScene...");
            SceneLoader.Instance.LoadGameSceneDirectly();
        }
    }
}