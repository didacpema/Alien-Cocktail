using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RecipeResult
{
    public string recipeName;
    public GameManager.OrderGrade grade;
    public int points;
    public float timeUsed;
    public bool wasSuccessful;
}

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance { get; private set; }

    [Header("Resultados")]
    private List<RecipeResult> completedOrders = new List<RecipeResult>();
    private int totalScore;
    private int totalOrders;

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
        RecipeResult result = new RecipeResult
        {
            recipeName = recipeName,
            grade = grade,
            points = points,
            timeUsed = timeUsed,
            wasSuccessful = wasSuccessful
        };

        completedOrders.Add(result);
        totalOrders++;
        
        Debug.Log($"[ResultManager] Resultado guardado: {recipeName} - {grade} - {points} puntos - Total órdenes: {totalOrders}");
        Debug.Log($"[ResultManager] Lista tiene {completedOrders.Count} elementos");
    }

    public void CalculateFinalResults(int finalScore)
    {
        totalScore = finalScore;
        Debug.Log($"Puntuación final: {totalScore}");
        Debug.Log($"Total de órdenes completadas: {totalOrders}");
        
        // Calcular estadísticas adicionales
        CalculateStatistics();
    }

    private void CalculateStatistics()
    {
        if (completedOrders.Count == 0) return;

        int successful = 0;
        int excellent = 0;
        int notable = 0;
        int good = 0;
        int sufficient = 0;
        int failed = 0;

        foreach (var order in completedOrders)
        {
            if (order.wasSuccessful) successful++;
            
            switch (order.grade)
            {
                case GameManager.OrderGrade.Excellent:
                    excellent++;
                    break;
                case GameManager.OrderGrade.Notable:
                    notable++;
                    break;
                case GameManager.OrderGrade.Good:
                    good++;
                    break;
                case GameManager.OrderGrade.Sufficient:
                    sufficient++;
                    break;
                case GameManager.OrderGrade.Failed:
                    failed++;
                    break;
            }
        }

        float successRate = (float)successful / completedOrders.Count * 100f;
        Debug.Log($"Tasa de éxito: {successRate:F1}%");
        Debug.Log($"Excelente: {excellent}, Notable: {notable}, Bueno: {good}, Suficiente: {sufficient}, Fallido: {failed}");
    }

    // Getters para la UI
    public List<RecipeResult> GetAllResults() => new List<RecipeResult>(completedOrders);
    public int GetTotalScore() => totalScore;
    public int GetTotalOrders() => totalOrders;
    public float GetSuccessRate() 
    {
        if (completedOrders.Count == 0) return 0f;
        int successful = 0;
        foreach (var order in completedOrders)
        {
            if (order.wasSuccessful) successful++;
        }
        return (float)successful / completedOrders.Count * 100f;
    }

    public void ClearResults()
    {
        completedOrders.Clear();
        totalScore = 0;
        totalOrders = 0;
    }
}