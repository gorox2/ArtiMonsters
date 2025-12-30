using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class smallImp : MonoBehaviour
{
    public float speed;
    public GameObject hit;
    public Collider2D detection;
    public LayerMask playerLayer;
    Transform playerPos;

    Animator anim;
    Rigidbody2D impRb;

    bool targetDetected;
    bool attack;
    int attackNum;

    float damage = 15f;
    float timeBetweenATKs = 1f;
    float time = 1;

    public Vector2 hitBoxSize;


    private void Start()
    {
        anim = GetComponent<Animator>();
        impRb = GetComponent<Rigidbody2D>();
        attackNum = 0;
    }

    private void Update()
    {
       attack = checkHitTrigger();


        
        if (targetDetected && playerPos != null && attack)
        {
            attackSecuence();
        }
    }

    private void FixedUpdate()
    {
        if (targetDetected && playerPos != null && !attack) 
        {
            Debug.Log("playerpos detected");
            Move();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("enter");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("player entered");
            targetDetected = true;
            playerPos = collision.gameObject.GetComponent<Transform>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("player exited");
            anim.SetBool("iswalk",false);
            targetDetected = false;
            playerPos = null;
        }
    }

    void Move()
    {
        anim.SetBool("iswalk", true);
        Vector2 direction = (playerPos.position - transform.position).normalized;
        impRb.velocity = new Vector2( direction.x * speed, impRb.velocity.y);

        if (transform.position.x - playerPos.position.x > 0)
        {
            transform.localScale = new Vector3(-4, 4, 1);
        }
        else if(transform.position.x - playerPos.position.x < 0)
        {
            transform.localScale = new Vector3(4, 4, 1);
        }
    }

    bool checkHitTrigger()
    {
        if (Physics2D.Raycast(transform.position, Vector2.left, 1f, playerLayer))
        {
            Debug.Log("supposed to hit");
            return true;
        }
        else 
        { 
            time = 1;
            return false;
        }
    }


    void attackSecuence()
    {
        anim.SetBool("iswalk", false);

        if(time >= timeBetweenATKs && attackNum ==0 )
        {
            Debug.Log("attack 1");
            anim.SetTrigger("atk1");
            attackNum = 1;
            time = 0;
            impRb.velocity =Vector2.zero;

        }
        else if(time >= timeBetweenATKs && attackNum == 1)
        {
            Debug.Log("attack 2");
            anim.SetTrigger("atk2");
            attackNum = 0;
            time = 0;
            impRb.velocity = Vector2.zero;

        }

    }


    public void AttackCooldown()
    {
         while (time < timeBetweenATKs && attack)
         {
            time += Time.deltaTime;
         }


    }



    public void dealDamageToPlayer(float damage)
    {
        Collider2D[] playerhit = Physics2D.OverlapBoxAll(hit.transform.position, hitBoxSize, 0, playerLayer);
        foreach (Collider2D player in playerhit)
        {
            if (player.CompareTag("Player"))
            {
                GameObject playerGO = player.gameObject;

                PlayerHealth playerHealth = playerGO.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeHit(damage);
                }
            }
        }
    }
}

