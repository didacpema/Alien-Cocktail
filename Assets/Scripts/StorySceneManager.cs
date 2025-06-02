using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StoryPhrase
{
    [TextArea(3, 10)]
    public string text;
    public float displayTime = 3f;
}

public class StorySceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Canvas storyCanvas;
    [SerializeField] private StoryPhrase[] storyPhrases;
    [SerializeField] private float typingSpeed = 30f;

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
        foreach (StoryPhrase phrase in storyPhrases)
        {
            if (storyText != null)
            {
                yield return StartCoroutine(TypeText(phrase.text));
                yield return new WaitForSeconds(phrase.displayTime);
            }
            else
            {
                yield break;
            }
        }

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadGameSceneDirectly();
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        storyText.text = "";

        foreach (char letter in textToType.ToCharArray())
        {
            storyText.text += letter;
            yield return new WaitForSeconds(1f / typingSpeed);
        }
    }
}