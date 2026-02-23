using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundScript : MonoBehaviour
{
    public float startPos, length;
    public GameObject cam;
    public float parallaxSpeed;

    private void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    private void FixedUpdate()
    {
        float movement = (cam.transform.position.x *(1- parallaxSpeed));
        float distance = ( cam.transform.position.x * parallaxSpeed);

        transform.position = new Vector3 (startPos +  distance, transform.position.y, transform.position.z);

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }

    }

}
