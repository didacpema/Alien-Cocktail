using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Stream : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3 targetPosition;

    private Coroutine pourCoroutine;
    public Transform originTransform; // Reference to pouring point
    public bool isPouring = false; // Flag to check if pouring
    public bool isHitting = false; // Flag to check if hitting something
    private Machine machine; // Reference to the machine object
    public Wobble wobble; // Reference to the Wobble script

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        machine = GameObject.FindGameObjectWithTag("Machine").GetComponent<Machine>();
    }

    void Start()
    {
        MoveToPosition(0, originTransform.position);
        MoveToPosition(1, transform.position);
    }
    void Update()
    {
        if (machine.isDrinking)
        {
            machine.animator.SetBool("isDrinking", true);
            if (!Machine.Instance.drinkSound.isPlaying)
            {
                Machine.Instance.drinkSound.Play();
                Machine.Instance.drinkSound.Stop(); // Opcional: si quieres que se detenga al dejar de beber

            }

        }
        else
        {
            machine.animator.SetBool("isDrinking", false);

        }
        if (gameObject.activeSelf && isPouring)
        {
            AnimateToPosition(0, originTransform.position);
            FindEndPoint();

            if (isHitting && HasReachedPosition(1, targetPosition))
            {
                machine.Fill(gameObject.tag);
                machine.isDrinking = true;
            }
            else
            {
                machine.isDrinking = false;
            }

            AnimateToPosition(1, targetPosition);
        }

        if (wobble.currentFillAmount <= -0.85f)
        {
            End();
        }
    }


    public void Begin()
    {
        pourCoroutine = StartCoroutine(BeginPour());
    }

    IEnumerator BeginPour()
    {
        while (gameObject.activeSelf)
        {
            isPouring = true;
            targetPosition = FindEndPoint();
            MoveToPosition(0, originTransform.position);
            AnimateToPosition(1, targetPosition);
            yield return null;
        }
    }

    public void End()
    {
        StopCoroutine(pourCoroutine);
        isPouring = false;
        machine.isDrinking = false; // << AÑADIDO
        pourCoroutine = StartCoroutine(EndPour());
    }

    private IEnumerator EndPour()
    {
        while (!HasReachedPosition(0, targetPosition))
        {
            AnimateToPosition(0, targetPosition);
            AnimateToPosition(1, targetPosition);
            yield return null;
        }

        Destroy(gameObject);
    }

    Vector3 FindEndPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(originTransform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, 50f))
        {
            // If we hit something, return the exact hit point
            //Debug.Log($"Hit object: {hit.collider.gameObject.name} on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Machine"))
            {            
                isHitting = true;
            }
            else
            {
                isHitting = false;
            }
            return hit.point;
        }
        else
        {
            // If we didn't hit anything, extend to the maximum distance
            return ray.GetPoint(15f);
        }
    }

    void MoveToPosition(int index, Vector3 targetPosition)
    {
        lineRenderer.SetPosition(index, targetPosition);
    }
    private void AnimateToPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPosition = lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPosition, Time.deltaTime * 15f);
        lineRenderer.SetPosition(index, newPosition);
    }
    private bool HasReachedPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPosition = lineRenderer.GetPosition(index);
        return currentPosition == targetPosition;
    }
    private IEnumerator UpdateParticle()
    {
        while(gameObject.activeSelf)
        {
            bool isHitting = HasReachedPosition(1, targetPosition);
            yield return null;
        }
    }
}
