using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredients : MonoBehaviour
{
    public static Ingredients Instance { get; private set; }
    public string ingredientName;
    private Machine machine;
    public Transform initialPosition;
    private float outOfBoundsDistance = 1.5f;
    public GameObject ingredientPrefab;
    private IngredientSpawner ingredientSpawner;

    void Start()
    {
        ingredientName = gameObject.tag;
        machine = GameObject.FindGameObjectWithTag("Machine").GetComponent<Machine>();
        ingredientSpawner = FindObjectOfType<IngredientSpawner>();
    }

    void Update()
    {
        //if gameobject goes out of bounds, reset its position
        if (Vector3.Distance(transform.position, initialPosition.position) > outOfBoundsDistance)
        {
            ResetPosition();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Machine"))
        {
            if (machine != null)
            {
                Machine.Instance.animator.SetTrigger("eat");
                machine.Fill(ingredientName);
                ResetPosition();
            }
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            ResetPosition();
        }
    }
    private void ResetPosition()
    {
        ingredientSpawner.SpawnIngredient(ingredientName, initialPosition);
        Destroy(gameObject);
    }
}
