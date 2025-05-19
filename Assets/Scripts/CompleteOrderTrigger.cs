using UnityEngine;

public class CompleteOrderTrigger : MonoBehaviour
{
    [Header("References")]
    public Transform drinkSnapPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Drink")) return;

        ClientManager clientManager = ClientManager.Instance;
        if (clientManager == null) return;

        HandleDrink(other.gameObject);

        clientManager.CompleteOrderWithDrink();
    }

    private void HandleDrink(GameObject drink)
    {
        drink.transform.position = drinkSnapPosition.position;
        drink.transform.rotation = drinkSnapPosition.rotation;

        Destroy(drink, 2f);
    }
}