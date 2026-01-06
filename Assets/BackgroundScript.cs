using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundScript : MonoBehaviour
{
    public GameObject playerT;
    public Transform[] background = new Transform [6];

    [SerializeField] float[] speed = new float[6];
    [SerializeField] float maxDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateBackground();

        for (int i = 0; i < background.Length; i++)
        {
            if (Mathf.Sqrt(Mathf.Pow(background[i].position.x, 2) -Mathf.Pow(playerT.transform.position.x,2)  ) >= maxDistance) 
            {
                moveBackgroundPos(background[i]);
            }
        }
    }

    void updateBackground()
    {
        if(playerT.GetComponent<Rigidbody2D>().linearVelocity.x >= 1)
        {
            for(int i = 0; i < speed.Length; i++)
            {
                background[i].Translate(-1* speed[i] * Time.deltaTime, 0, 0);
            }
        }
        else if(playerT.GetComponent<Rigidbody2D>().linearVelocity.x <= -1)
        {
            for(int i = 0; i < speed.Length; i++)
            {
                background[i].Translate(1* speed[i] * Time.deltaTime, 0, 0);
            }
        }
        else
        {
            for (int i = 0; i < speed.Length; i++)
            {
                background[i].Translate(0, 0, 0);
            }
        }
    }

    void moveBackgroundPos(Transform temp)
    {
        if(playerT.GetComponent<Rigidbody2D>().linearVelocity.x >= 1)
        {
            temp.position = new Vector2(temp.position.x + maxDistance,0);
        }
        else if (playerT.GetComponent<Rigidbody2D>().linearVelocity.x <= -1)
        {
            temp.position = new Vector2(temp.position.x - maxDistance, 0);
        }
    }
}
