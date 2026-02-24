
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class smallImp : MonoBehaviour
{
    const string WALK = "isWalking";
    const string ATTACK = "attack";
    const string DEAD = "isdead";
    const string HIT = "isHit";
    const string COOLDOWN = "isInCooldown";

    public enum State
    {
        Patrol,
        ChaseTarget,
        Attacking,
        Hurt,
        Death,
    }

    public State state;
    Animator animator;
    
    [SerializeField] HealthScript healthScript;

    public bool canMove;
    public bool isDead;
    public bool canAttack;

    [Header("Movement")]
    [SerializeField] float speed = 2f;
    Rigidbody2D rb;
    

    [Header("Detection")]
    public Transform playerPos;
    [SerializeField] float radius;
    [SerializeField] private float chaseStopDistance = 1.0f;

    [Header("Ledge Detection")]
    [SerializeField] Transform ledgeCheck;
    [SerializeField] Vector2 ledgeCheckSize;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool stopAtEdges = true;

    [Header("Combat")]
    [SerializeField] LayerMask playerLayer;
    public Transform hitPos;
    [SerializeField] Vector2 hitBoxSize;
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] float attackCooldown = 1f;
    bool isInCooldown;
    [SerializeField] float damage = 10f;

    [Header("Audio")]
    [SerializeField] AudioClip deathSound;
    [SerializeField] float deathSoundVolume = 1f;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] float hurtSoundVolume = 1f;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<HealthScript>();
    }

    private void OnEnable()
    {
        if (healthScript != null)
        {
            healthScript.onDeath += HandleDeath;
            healthScript.onDamaged += HandleHurt;
        }
        canMove = true;
        canAttack = true;
        isInCooldown = false;
        isDead = false;
        state = State.Patrol;
    }

   
    public void resetEnemy()
    {
        canMove = true;
        canAttack = true;
        isInCooldown = false;
        isDead = false;
        state = State.Patrol;
        rb.simulated = true;
    }
    

    private void Update()
    {
        if (state == State.Death) return;

       
        if (state == State.Attacking || state == State.Hurt)
        {
            animator.SetBool(WALK, false);
            return;
        }

        AcquireTargetIfNeeded();

        if (playerPos == null)
        {
            SetMove(0f);
            state = State.Patrol;
            return;
        }

        // Decide chase vs attack
        float dx = playerPos.position.x - transform.position.x;
        float absDx = Mathf.Abs(dx);
        float moveDir = Mathf.Sign(dx);
        if (absDx >= chaseStopDistance || !CanMoveInDirection(moveDir))
        {
            SetMove(0f);
            state = State.Patrol;
            return;
        }
        // Flip
        if (dx < -0.01f) SetFacing(-1f);
        else if (dx > 0.01f) SetFacing(+1f);

        // If close enough + in front => request an attack (animation locks the state)
        if (canAttack  && IsTargetInFront())
        {
            Attack();
            return;
        }

        // Chase
        state = State.ChaseTarget;
        
        SetMove(moveDir);
    }



    private void FixedUpdate()
    {
        if (state == State.Death || !canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
      
    }

    private void SetMove(float xDir)
    {
        if (!canMove) xDir = 0f;

        
        animator.SetBool(WALK, Mathf.Abs(xDir) > 0.01f);

        
        rb.linearVelocity = new Vector2(xDir * speed, rb.linearVelocity.y);
    }

    private void SetFacing(float sign)
    {
        
        float x = Mathf.Abs(transform.localScale.x) * Mathf.Sign(sign);
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }

    private bool HasGroundAheadOverlap(float dirSign)
    {
        Vector2 p = (Vector2)ledgeCheck.position + Vector2.right * dirSign * 0.15f;
        return Physics2D.OverlapBox(p, ledgeCheckSize, groundLayer) != null;
    }

    private bool CanMoveInDirection(float dirSign)
    {
        if (stopAtEdges && !HasGroundAheadOverlap(dirSign)) return false;
        
        return true;
    }

    private void AcquireTargetIfNeeded()
    {
        if (playerPos != null) return;

        Collider2D col = Physics2D.OverlapCircle(transform.position, radius, playerLayer);
        if (col != null) playerPos = col.transform;
    }

    private bool IsTargetInFront()
    {
        float facing = Mathf.Sign(transform.localScale.x);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(facing, 0f), attackRange, playerLayer);
        return hit.collider != null;
    }

    private void Attack()
    {
        if (state == State.Death) return;

        // Lock 
        state = State.Attacking;
        canMove = false;
        canAttack = false;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        animator.SetBool(WALK, false);
        animator.ResetTrigger(HIT);
        animator.SetTrigger(ATTACK);
    }
    
    public void dealDamageToPlayer()
    {
        Collider2D[] playerhit = Physics2D.OverlapBoxAll(hitPos.position, hitBoxSize, 0, playerLayer);
        foreach (Collider2D player in playerhit)
        {
            if (player.CompareTag("Player"))
            {
                Debug.Log("player hit confirmed... damage: " + damage);
                HealthScript health = player.GetComponent<HealthScript>();
                if (health == null) { Debug.Log("opponent doesnt have healthScript"); continue; }
                health.takeDamage(damage);
            }
        }
    }


    public void HandleHurt(float dmg)
    {
        if (state == State.Death) return;

        
        if (state == State.Hurt) return;

        
        state = State.Hurt;
        canMove = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        animator.SetBool(WALK, false);
        animator.ResetTrigger(ATTACK);
        animator.SetTrigger(HIT);

        SoundFXManager.Instance.PlaySoundFXClip(hurtSound, transform, hurtSoundVolume);
    }
    public void RecoverFromHurt()
    {
        if (!isDead)
        {
            canMove = true;

            canAttack = true;

            state = State.Patrol;
        }
    }

    
    public void HandleDeath()
    {
        if (state == State.Death) return;
        isDead = true;

        
        state = State.Death;

        
        rb.linearVelocity = Vector2.zero;

        
        rb.simulated = false; 

        animator.ResetTrigger(ATTACK);
        animator.ResetTrigger(HIT);
        animator.SetBool(WALK, false);
        animator.SetTrigger(DEAD);

        SoundFXManager.Instance.PlaySoundFXClip(deathSound, gameObject.transform, deathSoundVolume);
        
    }


    public void AttackEnd()
    {
        if (state == State.Death) return;

        canMove = true;
        canAttack = true;

        
        state = State.Patrol;
    }



    public void disableGO()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (healthScript != null)
        {
            healthScript.onDeath -= HandleDeath;
            healthScript.onDamaged -= HandleHurt;
        }

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawCube(hitPos.position, hitBoxSize);

        if (ledgeCheck != null)
            Gizmos.DrawCube(ledgeCheck.position, ledgeCheckSize);

    }
}


