using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance { get; private set; }

    [Header("UI Reference")]
    public Text resultsText; // Asignar desde el inspector

    private int totalScore = 0;
    private int ordersCompleted = 0;
    private List<string> recipeNames = new List<string>(); // Para guardar nombres de recetas

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveRecipeResult(string recipeName, GameManager.OrderGrade grade, int points, float timeUsed, bool wasSuccessful)
    {
        ordersCompleted++;
        recipeNames.Add(recipeName); // Solo guardar el nombre de la receta
        
        Debug.Log($"[ResultManager] Guardado: {recipeName} - {points} puntos");
        // NO agregamos nada al texto aquí, solo guardamos los datos
    }

    public void CalculateFinalResults(int finalScore)
    {
        totalScore = finalScore;
        
        // Mostrar todas las recetas preparadas
        string allRecipes = "";
        if (recipeNames.Count > 0)
        {
            allRecipes = "Recetas preparadas: " + string.Join(", ", recipeNames) + "\n\n";
        }
        
        // Crear SOLO el resumen final
        string finalText = $"=== RESUMEN DEL TURNO ===\n\n";
        finalText += allRecipes;
        finalText += $"PUNTUACIÓN TOTAL: {totalScore} puntos\n";
        finalText += $"ÓRDENES COMPLETADAS: {ordersCompleted}\n";

        // Mostrar en la UI
        if (resultsText != null)
        {
            resultsText.text = finalText;
        }

        Debug.Log("=== TURNO TERMINADO ===");
        Debug.Log(finalText);
    }

    private string GetGradeText(GameManager.OrderGrade grade)
    {
        return grade switch
        {
            GameManager.OrderGrade.Excellent => "Excelente",
            GameManager.OrderGrade.Good => "Bueno",
            GameManager.OrderGrade.Failed => "Suspendido",
            _ => "Desconocido"
        };
    }

    public void ClearResults()
    {
        totalScore = 0;
        ordersCompleted = 0;
        recipeNames.Clear();
        
        if (resultsText != null)
        {
            resultsText.text = "";
        }
    }

    // Métodos públicos para acceder a los datos si necesitas
    public int GetTotalScore() => totalScore;
    public int GetOrdersCompleted() => ordersCompleted;
    public List<string> GetRecipeNames() => new List<string>(recipeNames);
}