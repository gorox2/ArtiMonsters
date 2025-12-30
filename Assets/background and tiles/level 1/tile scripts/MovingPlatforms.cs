using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] float platSpeed;
    bool istriggered = false;
    bool goingUp = true;
    private void FixedUpdate()
    {
        if (istriggered && goingUp)
        {
            transform.Translate(0,platSpeed,0);
        }
        else if (istriggered && !goingUp)
        {
            transform.Translate(0,-platSpeed,0);    
        }

        if(transform.position.y >= 6)
        {
            goingUp = false;
        }
        else if(transform.position.y <= 0)
        {
            goingUp = true;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            istriggered = true;   
        }
    }

    
}
