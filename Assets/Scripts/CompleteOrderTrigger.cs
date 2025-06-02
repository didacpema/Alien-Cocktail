using UnityEngine;
using UnityEngine.XR;

public class CompleteOrderTrigger : MonoBehaviour
{
    [Header("References")]
    public Transform drinkSnapPosition;
    private ClientManager clientManager;
    private string DrinkName;

    private void Start()
    {
        clientManager = ClientManager.Instance;
        if (clientManager == null) return;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag("Drink"))
        {
            HandleDrink(other.gameObject);
            clientManager.CompleteOrderWithDrink();
            return;
        }
        else
        { 
            Debug.LogWarning($"Drink {other.name} does not match the expected drink: {DrinkName}");
            return;
        }

    }
    private void HandleDrink(GameObject drink)
    {
        drink.transform.position = drinkSnapPosition.position;
        drink.transform.rotation = drinkSnapPosition.rotation;

        Destroy(drink, 2f);
    }
}