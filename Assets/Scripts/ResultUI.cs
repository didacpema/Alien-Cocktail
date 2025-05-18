using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text totalScoreText;
    public TextMeshPro successRateText;
    public TextMeshPro totalOrdersText;
    public Text allResultsText;

    [Header("Buttons")]
    public Button playAgainButton;
    public Button mainMenuButton;

    private void Start()
    {
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(PlayAgain);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        DisplayResults();
    }

    private void DisplayResults()
    {
        if (ResultManager.Instance == null)
        {
            Debug.LogError("ResultManager.Instance es null!");
            return;
        }

        // Mostrar estadísticas generales
        if (totalScoreText != null)
            totalScoreText.text = $"Puntuación Total: {ResultManager.Instance.GetTotalScore()}";

        if (successRateText != null)
            successRateText.text = $"Tasa de Éxito: {ResultManager.Instance.GetSuccessRate():F1}%";

        if (totalOrdersText != null)
            totalOrdersText.text = $"Órdenes Completadas: {ResultManager.Instance.GetTotalOrders()}";

        // Mostrar todos los resultados en un solo texto
        DisplayAllResults();
    }

    private void DisplayAllResults()
    {
        if (allResultsText == null)
        {
            Debug.LogWarning("AllResultsText no está asignado");
            return;
        }

        List<RecipeResult> results = ResultManager.Instance.GetAllResults();
        string resultText = "RESULTADOS DETALLADOS:\n\n";

        for (int i = 0; i < results.Count; i++)
        {
            RecipeResult result = results[i];
            resultText += $"Orden {i + 1}:\n";
            resultText += $"  Receta: {result.recipeName}\n";
            resultText += $"  Calificación: {GetGradeText(result.grade)}\n";
            resultText += $"  Puntos: {result.points}\n";
            resultText += $"  Tiempo usado: {result.timeUsed:F1}s\n";
            resultText += $"  Éxito: {(result.wasSuccessful ? "Sí" : "No")}\n\n";
        }

        if (results.Count == 0)
        {
            resultText += "No hay resultados para mostrar.\n";
        }

        allResultsText.text = resultText;
    }

    private string GetGradeText(GameManager.OrderGrade grade)
    {
        return grade switch
        {
            GameManager.OrderGrade.Excellent => "Excelente",
            GameManager.OrderGrade.Notable => "Notable",
            GameManager.OrderGrade.Good => "Bueno",
            GameManager.OrderGrade.Sufficient => "Suficiente",
            GameManager.OrderGrade.Failed => "Fallido",
            _ => "Desconocido"
        };
    }

    private Color GetGradeColor(GameManager.OrderGrade grade)
    {
        return grade switch
        {
            GameManager.OrderGrade.Excellent => Color.green,
            GameManager.OrderGrade.Notable => Color.cyan,
            GameManager.OrderGrade.Good => Color.yellow,
            GameManager.OrderGrade.Sufficient => new Color(1f, 0.5f, 0f), 
            GameManager.OrderGrade.Failed => Color.red,
            _ => Color.white
        };
    }

    private void PlayAgain()
    {
        // Limpiar resultados para el siguiente juego
        if (ResultManager.Instance != null)
            ResultManager.Instance.ClearResults();

        // Cargar la escena del juego
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadGameScene();
    }

    private void GoToMainMenu()
    {
        // Limpiar resultados
        if (ResultManager.Instance != null)
            ResultManager.Instance.ClearResults();

        // Cargar el menú principal
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadMainMenu();
    }
} 