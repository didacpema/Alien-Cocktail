using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Machine : MonoBehaviour
{
    public float fillAmount = 0f;
    public TextMeshProUGUI fillAmountText; 
    void Start()
    {

    }

    void Update()
    {
        fillAmountText.text = "Fill Amount: " + fillAmount.ToString("F2"); // Update the UI text with the fill amount
    }
    public float GetMachineFillAmount()
    {
        return fillAmount;
    }
    public float Fill()
    {
        fillAmount += Time.deltaTime;
        return fillAmount;
    }
}
