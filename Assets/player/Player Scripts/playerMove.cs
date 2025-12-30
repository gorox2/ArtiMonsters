using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerMove : MonoBehaviour
{
     Rigidbody2D playerRb;
    Animator anim;
    TrailRenderer dashtrail;

    float moveDir;
    public float speed;
    public float jumpforce;
    public float dashforce;

    public Vector2 rayBoxSize;
    public float castDistance;
    public LayerMask groundLayer;

   public static bool onGround;
    bool can2jump = true;
    bool canDash = true;
    float dashtimer = 0;

   public AnimationClip frame;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>(); 
        anim = GetComponent<Animator>();
        dashtrail = GetComponent<TrailRenderer>();  
    }

    // Update is called once per frame
    void Update()
    {
        run();
        death();
        grounded();
        jump();
        dash();
        knockback();
    }

    private void FixedUpdate()
    {
        playerRb.velocity = new Vector2((moveDir * speed) + dashforce, playerRb.velocity.y);
        
    }

    void run()
    {
        moveDir = Input.GetAxis("Horizontal");
        if (moveDir > 0.1f)
        { 
            transform.localScale = new Vector2(2,2);
            if (onGround) { anim.SetBool("isrun", true); }
        }
        else if (moveDir < -0.1f)
        {
            transform.localScale = new Vector2(-2,2);
            if (onGround) { anim.SetBool("isrun", true); }
        }
        else
        {
            anim.SetBool("isrun", false);
        }
    }

    void jump()
    {
        if (onGround == true && Input.GetButtonDown("Jump"))
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpforce);
            onGround = false;
            dashforce = 0;
            anim.SetBool("isjump", true);
            anim.SetBool("Djump", false);
        }

        else if (onGround == false && can2jump && Input.GetButtonDown("Jump"))
        {
            can2jump = false;
            dashforce = 0;
            if (!can2jump)
            {
                anim.SetBool("Djump", true);
            }
            
            anim.SetBool("isjump", false);
            anim.SetBool("isfall", false);
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpforce * 0.7f);         
        }
    }

    void grounded()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(transform.position, rayBoxSize, 0, -transform.up, castDistance, groundLayer);
        if (raycast.collider != null)
        {
            onGround = true;
            can2jump = true;

            anim.SetBool("isfall", false);
            anim.SetBool("isjump", false);
        }
        else
        {
            onGround = false; 
                  
            anim.SetBool("isrun", false);
            if (anim.GetBool("Djump") == false)
            {
                anim.SetBool("isfall", true);
            }
        }

    }

    public void fall()
    {
        anim.SetBool("isjump", false);
        anim.SetBool("isfall", true);
        anim.SetBool("Djump", false);

    }

    

    void dash()
    {
        if (canDash == true  &&  Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            dashtrail.emitting = true;
            anim.SetBool("isrun", false);
            if (transform.localScale.x > 0)
            {
                dashforce = 10f;
                
            } 
            if (transform.localScale.x < 0)
            {
                dashforce = -10f;
                
            }
            if (onGround)
            {
                anim.SetBool("isdash", true);

            }
            else { anim.SetBool("airdash", true); }
        }
        
    }

    public void stopdash()
    {
        dashforce = 0;
        anim.SetBool("isdash", false);
        anim.SetBool("airdash", false);
        canDash = false;
        dashtrail.emitting = false;
        while (dashtimer < 1)
        {
            dashtimer += Time.deltaTime;
        }
        if (dashtimer > 0)
        {
            canDash = true;
            dashtimer = 0;
        }
        

    }
    void knockback()
    {

    }

    void death()
    {
        
    }

    

    
}
