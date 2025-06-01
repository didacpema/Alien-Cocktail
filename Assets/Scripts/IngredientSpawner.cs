using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    //list of prefabs to spawn
    public List<GameObject> ingredientPrefabs;
    //list of spawn positions
    public List<Transform> spawnPositions;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SpawnIngredient(string ingredientName, Transform spawnPosition)
    {
        //find the prefab with the given name
        GameObject ingredientPrefab = ingredientPrefabs.Find(prefab => prefab.name == ingredientName);
        if (ingredientPrefab != null)
        {
            //instantiate the prefab at the spawn position
            Instantiate(ingredientPrefab, spawnPosition.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Ingredient prefab not found: " + ingredientName);
        }
    }
}
