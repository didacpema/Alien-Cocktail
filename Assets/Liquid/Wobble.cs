using UnityEngine;

public class Wobble : MonoBehaviour
{
    Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;  
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;
    
    [Header("Liquid Settings")]
    public float maxFillAmount = 1f;
    public float currentFillAmount = 1f;
    public float pourRate = 0.05f;
    private bool isPouring = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        UpdateShaderFillAmount();
    }

    private void Update()
    {
        time += Time.deltaTime;
        
        // Wobble calculations (existing code)
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * Recovery);
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * Recovery);

        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        // Velocity calculations
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;

        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;

        // Liquid emptying logic
        if (isPouring)
        {
            currentFillAmount -= pourRate * Time.deltaTime;
            currentFillAmount = Mathf.Clamp(currentFillAmount, -0.85f, maxFillAmount);
            UpdateShaderFillAmount();
        }
    }

    public void StartPouring()
    {
        isPouring = true;
    }

    public void StopPouring()
    {
        isPouring = false;
    }

    public void RefillBottle()
    {
        currentFillAmount = maxFillAmount;
        UpdateShaderFillAmount();
    }

    private void UpdateShaderFillAmount()
    {
        rend.material.SetFloat("_Fill", currentFillAmount);
    }
}