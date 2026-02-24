using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerMove : MonoBehaviour
{
    const string MOVEMENT = "movement";
    const string INAIR = "inAir";
    const string DEAD = "isdead";
    const string HIT = "isHit";
    const string COOLDOWN = "isInCooldown";


    Animator animator;
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float airAcceleration = 20f;
    public float airMoveSpeed = 8f;
    public float jumpForce = 12f;
    public float dashForce = 10f;
    public float dash = 0f;
    public float dashTime = 0.2f;
    public float dashCooldown = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;
    public float jumpUngroundTime = 0.12f;  
    float jumpUngroundTimer;

    [Header("Slope")]
    public float stickToGroundForce = 8f;
    public float maxSlopeAngle = 60f;

    Rigidbody2D rb;

    public bool isGrounded;
    bool jumpedThisFrame;
    bool canDash;

    Vector2 slopeNormal = Vector2.up;
    PlayerAttack playerAttack;
    MovingPlatform2D groundPlatform;

    public bool canMove;
    float moveInput;

    void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
    }
    private void Start()
    {
        canDash = true;
        canMove = true;
    }
    void Update()
    {
        if (!canMove)
        {
            animator.SetFloat(MOVEMENT, 0f);
            return;
        }
        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0) { transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveInput, transform.localScale.y, transform.localScale.z); }
        if (Input.GetButtonDown("Jump") && !playerAttack.isAttacking)
            Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0f,rb.linearVelocity.y) ;
            return;
        }
        if (jumpUngroundTimer > 0f)
            jumpUngroundTimer -= Time.fixedDeltaTime;

        CheckGround();


        // if press jump button - ignore movement for first frame, otherwise it will stick to the slope
        if (jumpedThisFrame)
        {
            jumpedThisFrame = false;
            
        }

        // handle movement, including sticking to slope
        HandleMovement();

        
    }

    // ---------------- GROUND CHECK ----------------

    void CheckGround()
    {
        // ignore regrounding for a short time after jump starts
        if (jumpUngroundTimer > 0f)
        {
            isGrounded = false;
            groundPlatform = null;
            slopeNormal = Vector2.up;
            //animator.SetBool(INAIR, true);
            return;
        }

        Collider2D col = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

       
        if (col)
        {
            isGrounded = true;
            animator.SetBool("inAir", false);
            groundPlatform = col.GetComponent<MovingPlatform2D>();
            if (groundPlatform == null)
                groundPlatform = col.GetComponentInParent<MovingPlatform2D>();


            // if on ground - make raycast to it to find the normal of the ground
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.6f, groundLayer);

            if (hit)
                slopeNormal = hit.normal;
        }
        else
        {
            isGrounded = false;
            groundPlatform = null;
            slopeNormal = Vector2.up;
            animator.SetBool("inAir", true);
        }
    }

    // ---------------- MOVEMENT ----------------

    void HandleMovement()
    {
        Vector2 platformDelta = Vector2.zero;

        // if player on moving platform - use the velocity of the platform as offset for the player's velocity 
        if (groundPlatform != null)
            platformDelta = groundPlatform.Velocity;

        Vector2 v = rb.linearVelocity;

        if (isGrounded)
        {
            //move according to input on x + velocity offset from platform
            float x = moveInput * moveSpeed  + dash;
            v.x = x + platformDelta.x;
            // maintain the velocity on y (because the player is on the platform and moves with it already)
            if (jumpUngroundTimer <= 0f)
                if (platformDelta.y > v.y)
                    v.y = platformDelta.y;

            rb.linearVelocity = v;

            animator.SetFloat("movement", Mathf.Abs(moveInput));

            float slopeAngle = Vector2.Angle(slopeNormal, Vector2.up);
            bool onWalkableSlope = slopeAngle > 0.1f && slopeAngle <= maxSlopeAngle;
            // Stick player slightly to slope by adding force every frame - in opposite direction of slope normal (it pushes against the slope)
            if (jumpUngroundTimer <= 0f && onWalkableSlope)
            {
                Debug.Log("applying slope force");
                
                Vector2 intoSlope = -slopeNormal * (stickToGroundForce);
                rb.AddForce(intoSlope, ForceMode2D.Force);
            }
        }
        else
        {
            //move according to input on x + velocity offset from platform
            v.x += moveInput * airAcceleration * Time.fixedDeltaTime;
            v.x = Mathf.Clamp(v.x, -airMoveSpeed, airMoveSpeed);

            // maintain the velocity on y (because the player is on the platform and moves with it already)
            float y = rb.linearVelocity.y;


            //rb.linearVelocity = v.x < 0 ? new Vector2(v.x + (Time.fixedDeltaTime * airMoveSpeed), y) : new Vector2(v.x - (Time.fixedDeltaTime * airMoveSpeed), y);
            rb.linearVelocity = v;

        }

    }

    // ---------------- JUMP ----------------

    void Jump()
    {
        // prevent jumping if is in the air
        if (!isGrounded)
            return;
        
        jumpedThisFrame = true;
        isGrounded = false;
        groundPlatform = null;

        jumpUngroundTimer = jumpUngroundTime;

        // maintain the velocity on x when jumping
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        // give impulse upward
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        animator.SetTrigger("jump");
    }

    // ---------------- DEBUG ----------------

    // gizmos will be shown in scene tab, if select the player
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;

        // will show sphere collider that detects the collision of player's feet 
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // will show the normal from the ground
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + (Vector3)slopeNormal);
    }

    bool IsSlopeWalkable()
    {
        float angle = Vector2.Angle(slopeNormal, Vector2.up);
        return angle > 0.1f && angle <= maxSlopeAngle;
    }

    void Dash()
    {
        if (!isGrounded) return;

        if (canDash)
        {
            
            animator.SetTrigger("dash");
            StartCoroutine(Dashing());
        }

    }

    IEnumerator Dashing()
    {
        canDash = false;
        float timer = 0f;
        while (timer <= dashTime)
        {
            timer += Time.deltaTime;
            dash = transform.localScale.x * dashForce;
            yield return null;
        }
        timer = 0f;
        dash = 0;
        while (timer <= dashCooldown)
        {  
            timer += Time.deltaTime;
            yield return null;
        }
        canDash = true;
    }



}


