using UnityEngine;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class IngredientObject : MonoBehaviour
{
    public int ingredientIndex;
    public IngredientType type;
    
    private Rigidbody rb;
    private ParticleSystem pourEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pourEffect = GetComponentInChildren<ParticleSystem>();
        ConfigurePhysics();
    }

    private void ConfigurePhysics()
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Update()
    {
        if (type == IngredientType.Liquid)
        {
            CheckPouring();
        }
    }

    private void CheckPouring()
    {
        float pourAngle = Vector3.Angle(transform.up, Vector3.up);
        bool isPouring = pourAngle > pourThreshold && 
                        Physics.Raycast(transform.position, Vector3.down, maxPourDistance);
        
        if (isPouring && !pourEffect.isPlaying)
        {
            pourEffect.Play();
        }
        else if (!isPouring && pourEffect.isPlaying)
        {
            pourEffect.Stop();
        }
    }

    public Ingredient GetIngredientData()
    {
        return RecipeManager.Instance.availableIngredients[ingredientIndex];
    }
}