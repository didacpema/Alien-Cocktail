﻿using UnityEngine;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = 45;
    public Transform originTransform;
    public GameObject streamPrefab;

    private bool isPouring = false;
    private Stream currentStream;
    public Wobble wobble;

    void Start()
    {
    }

    void Update()
    {
        bool pourCheck = CalculatePourAngle() > pourThreshold;

        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;
            if (isPouring && wobble.currentFillAmount > 0.1f)
                StartPour();
            else
                EndPour();
        }
    }

    float CalculatePourAngle()
    {
        float zAngle = 180-Mathf.Abs(180-transform.rotation.eulerAngles.z); 
        float xAngle = 180 - Mathf.Abs(180 - transform.rotation.eulerAngles.x);
        return Mathf.Max(zAngle, xAngle);
    }

    void StartPour()
    {

        
        currentStream = CreateStream();
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
}