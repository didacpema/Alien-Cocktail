using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanRotation : MonoBehaviour
{
    [Tooltip("Velocidad de rotación en grados por segundo")]
    public float rotationSpeed = 360f;

    void Update()
    {
        // Rota sobre su propio eje Y (local)
        transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime, Space.Self);
    }
}

