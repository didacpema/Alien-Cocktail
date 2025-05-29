using UnityEngine;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = 45;
    public Transform originTransform;
    public GameObject streamPrefab;

    private bool isPouring = false;
    private Stream currentStream;
    public Wobble wobble;
    public Transform initialPosition; 
    private float outOfBoundsDistance = 1.5f;

    void Start()
    {
    }

    void Update()
    {
        bool pourCheck = CalculatePourAngle() > pourThreshold;
        if (Vector3.Distance(transform.position, initialPosition.position) > outOfBoundsDistance)
        {
            ResetPosition();
        }

        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;
            if (isPouring && wobble.currentFillAmount > -0.85f)
                StartPour();
            else
                EndPour();
        }
    }

    float CalculatePourAngle()
    {
        float zAngle = 180 - Mathf.Abs(180 - transform.rotation.eulerAngles.z);
        float xAngle = 180 - Mathf.Abs(180 - transform.rotation.eulerAngles.x);
        return Mathf.Max(zAngle, xAngle);
    }

    void StartPour()
    {


        currentStream = CreateStream();
        currentStream.GetComponent<Renderer>().material.color = wobble.GetComponent<Renderer>().material.color;
        //change the tag of the stream to match the wobble
        currentStream.gameObject.tag = wobble.gameObject.tag;
        currentStream.Begin();
        wobble.StartPouring();
        Debug.Log("Pour Started");

    }

    void EndPour()
    {
        currentStream.End();
        currentStream = null;
        wobble.StopPouring();
        Debug.Log("Pour Ended");
    }

    private Stream CreateStream()
    {
        GameObject stream = Instantiate(streamPrefab, originTransform.position, Quaternion.identity);
        Stream streamScript = stream.GetComponent<Stream>();
        streamScript.originTransform = originTransform;
        streamScript.wobble = wobble;
        return streamScript;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Barra"))
        {
            wobble.RefillBottle();
            if (currentStream != null)
            {
                EndPour();
            }
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            ResetPosition();
        }
    }
    private void ResetPosition()
    {
        transform.position = initialPosition.position;
        transform.rotation = initialPosition.rotation;
        GetComponent<Rigidbody>().velocity = Vector3.zero; // Reset velocity
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // Reset angular velocity
    }
}