using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAnimationScript : MonoBehaviour
{
    public Animator alienAnimator;
    public float animationSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ClientManager.Instance.isMoving)
        {
            alienAnimator.SetBool("isMoving", true);
            //alienAnimator.speed = animationSpeed;
        }
        else
        {
            alienAnimator.SetBool("isMoving", false);
            //alienAnimator.speed = 0.0f; // Stop the animation when not moving
        }
    }
}
