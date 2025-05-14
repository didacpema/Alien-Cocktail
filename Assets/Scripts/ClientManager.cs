using UnityEngine;
using System.Collections;

public class ClientManager : MonoBehaviour
{
    [Header("Client Settings")]
    public GameObject alienPrefab;
    public float moveSpeed = 1.5f;
    public float timeBetweenClients = 10f;
    public float waitTimeAtDestination = 5f; 
    public int maxActiveClients = 1;

    [Header("References")]
    private Transform spawnPoint;
    private Transform destinationPoint;
    private GameObject currentAlien;

    private void Awake()
    {
        GameObject spawnObj = GameObject.FindGameObjectWithTag("SpawnPoint");
        GameObject destObj = GameObject.FindGameObjectWithTag("Destination");

        if (spawnObj != null) spawnPoint = spawnObj.transform;
        if (destObj != null) destinationPoint = destObj.transform;

        Debug.Log($"Spawn: {spawnPoint != null}, Dest: {destinationPoint != null}");
    }

    public void GenerateNewClient()
    {
        if (currentAlien == null && maxActiveClients > 0)
        {
            StartCoroutine(SpawnMoveAndReturnClient());
        }
    }

    private IEnumerator SpawnMoveAndReturnClient()
    {
        Debug.Log("Generando alien..."); // Paso 1
        currentAlien = Instantiate(alienPrefab, spawnPoint.position, spawnPoint.rotation);

        Debug.Log("Moviendo hacia destino: " + destinationPoint.position); // Paso 2
        yield return MoveToPosition(destinationPoint.position);

        Debug.Log("Esperando en destino..."); // Paso 3
        yield return new WaitForSeconds(waitTimeAtDestination);

        Debug.Log("Regresando al spawn..."); // Paso 4
        yield return MoveToPosition(spawnPoint.position);

        Debug.Log("Destruyendo alien"); // Paso 5
        Destroy(currentAlien);
        currentAlien = null;

        yield return new WaitForSeconds(timeBetweenClients);
        GenerateNewClient();
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(currentAlien.transform.position, targetPosition) > 0.1f)
        {
            currentAlien.transform.position = Vector3.MoveTowards(
                currentAlien.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }
}