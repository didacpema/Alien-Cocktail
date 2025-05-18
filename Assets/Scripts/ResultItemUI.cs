using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI timeText;
    public Image backgroundImage;
    public Image gradeIcon;

    [Header("Grade Icons (Opcional)")]
    public Sprite excellentIcon;
    public Sprite notableIcon;
    public Sprite goodIcon;
    public Sprite sufficientIcon;
    public Sprite failedIcon;

    public void SetupResult(RecipeResult result)
    {
        // Configurar textos
        if (recipeNameText != null)
            recipeNameText.text = result.recipeName;

        if (gradeText != null)
        {
            gradeText.text = GetGradeText(result.grade);
            gradeText.color = GetGradeColor(result.grade);
        }

        if (pointsText != null)
            pointsText.text = $"{result.points} pts";

        if (timeText != null)
            timeText.text = $"Tiempo: {result.timeUsed:F1}s";

        // Configurar fondo
        if (backgroundImage != null)
        {
            Color bgColor = GetGradeColor(result.grade);
            bgColor.a = 0.2f; // Hacer el fondo más transparente
            backgroundImage.color = bgColor;
        }

        // Configurar icono de calificación
        if (gradeIcon != null)
        {
            gradeIcon.sprite = GetGradeIcon(result.grade);
            gradeIcon.color = GetGradeColor(result.grade);
        }
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
            GameManager.OrderGrade.Excellent => new Color(0.2f, 0.8f, 0.2f), // Verde
            GameManager.OrderGrade.Notable => new Color(0.2f, 0.8f, 0.8f),   // Cyan
            GameManager.OrderGrade.Good => new Color(1f, 1f, 0.2f),          // Amarillo
            GameManager.OrderGrade.Sufficient => new Color(1f, 0.6f, 0.2f),  // Naranja
            GameManager.OrderGrade.Failed => new Color(0.8f, 0.2f, 0.2f),    // Rojo
            _ => Color.white
        };
    }

    private Sprite GetGradeIcon(GameManager.OrderGrade grade)
    {
        return grade switch
        {
            GameManager.OrderGrade.Excellent => excellentIcon,
            GameManager.OrderGrade.Notable => notableIcon,
            GameManager.OrderGrade.Good => goodIcon,
            GameManager.OrderGrade.Sufficient => sufficientIcon,
            GameManager.OrderGrade.Failed => failedIcon,
            _ => null
        };
    }
}