using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum BossState { Inactive, Appearing, Idle, Walk, Attacking, Casting, Hurt, Dead }

    public BossState state = BossState.Inactive;

    [Header("Info")]
    [SerializeField] string bossName = "Boss";
    public string BossName => bossName;

    public HealthScript Health { get; private set; }

    [Header("Refs")]
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform player;
    [SerializeField] Transform castPoint;
    [SerializeField] BossFacing facing;
    [SerializeField] BossAnimEvents animEvents;
    public BossAnimEvents AnimEvents => animEvents;
    public event Action OnDied;

    [Header("AI")]
    [SerializeField] float decideEverySeconds = 1.0f;
    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float stopDistance = 1.8f;
    [SerializeField] float chaseDistance = 6.0f;

    [Header("Attacks")]
    [SerializeField] BossAttackBase[] attacks;
    public Transform hitPoint;

    [Header("Audio")]
    [SerializeField] AudioClip deathSound;
    [SerializeField] float deathSoundVolume = 1f;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] float hurtSoundVolume = 1f;
    [SerializeField] AudioClip appearSound;
    [SerializeField] float appearSoundVolume = 1f;

    public Transform Player => player;
    public Transform CastPoint => castPoint;
    public Animator Animator => animator;

    public bool IsFacingRight => facing != null && facing.IsFacingRight();
    public bool HasFightBegun { get; private set; }
    
    Coroutine aiRoutine;

    Dictionary<BossAttackBase, float> lastUse = new();

    private void Awake()
    {
        Health = GetComponent<HealthScript>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (facing == null) facing = GetComponent<BossFacing>();
        if (animEvents == null) animEvents = GetComponent<BossAnimEvents>();
    }

    private void OnEnable()
    {
        state = BossState.Inactive;
        HasFightBegun = false;
        if (Health != null)
        {
            Health.onDamaged += OnDamaged;
            Health.onDeath += OnDeath;
        }
    }

    private void OnDisable()
    {
        if (Health != null)
        {
            Health.onDamaged -= OnDamaged;
            Health.onDeath -= OnDeath;
        }
    }

    private void FixedUpdate()
    {
        if (state == BossState.Dead || state == BossState.Appearing || state == BossState.Hurt ||
        state == BossState.Attacking || state == BossState.Casting)
        {
            animator.SetBool("Moving", false);
            return;
        }

        if (player == null) return;

        
        facing?.FaceTarget(player.position);

        float dist = Vector2.Distance(rb.position, player.position);

        if (dist > stopDistance && dist <= chaseDistance)
        {
            animator.SetBool("Moving", true);

            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);

            state = BossState.Walk;
        }
        else
        {
            animator.SetBool("Moving", false);
            state = BossState.Idle;
        }
    }

    public void StartBossFight()
    {
        if (state != BossState.Inactive) return;

        state = BossState.Appearing;
        animator.SetTrigger("Appear");
        SoundFXManager.Instance.PlaySoundFXClip(appearSound, transform, appearSoundVolume);

    }

    
    public void OnAppearFinished()
    {
        if (state == BossState.Dead) return;
        HasFightBegun = true;
        state = BossState.Idle;

        if (aiRoutine != null) StopCoroutine(aiRoutine);
        aiRoutine = StartCoroutine(AILoop());
    }

    IEnumerator AILoop()
    {
        while (state != BossState.Dead)
        {
            yield return new WaitForSeconds(decideEverySeconds);

            if (state == BossState.Idle || state == BossState.Walk)
                TryChooseAndExecuteAttack();
        }
    }

    void TryChooseAndExecuteAttack()
    {
        if (attacks == null || attacks.Length == 0) return;

        BossAttackBase chosen = BossAttackSelector.Choose(attacks, this);
        if (chosen == null) return;

        
        StartCoroutine(ExecuteAttack(chosen));
    }

    IEnumerator ExecuteAttack(BossAttackBase attack)
    {
        if (state == BossState.Dead) yield break;

        // lock state based on attack type
        state = attack.attackType == BossAttackBase.AttackType.Cast ? BossState.Casting : BossState.Attacking;

        yield return attack.Execute(this);

        // back to idle/walk
        if (state != BossState.Dead)
        {
            state = BossState.Idle;
            animator.SetBool("Moving", false);
        }
    }

    public bool IsAttackOffCooldown(BossAttackBase atk)
    {
        if (!lastUse.TryGetValue(atk, out float t)) return true;
        return Time.time >= t + atk.cooldown;
    }

    public void MarkAttackUsed(BossAttackBase atk)
    {
        lastUse[atk] = Time.time;
    }
    void OnDamaged(float dmg)
    {
        if (state == BossState.Dead) return;

        if (state == BossState.Idle || state == BossState.Walk)
        {
            if (aiRoutine != null) StopCoroutine(aiRoutine);
            StartCoroutine(HurtRoutine());
        }
        
    }

    IEnumerator HurtRoutine()
    {
        state = BossState.Hurt;
        animator.SetTrigger("Hurt");
        SoundFXManager.Instance.PlaySoundFXClip(hurtSound,transform,hurtSoundVolume);
        
        yield return new WaitUntil(() => !IsInState("Hurt"));

        if (state != BossState.Dead)
        {
            state = BossState.Idle;
            aiRoutine = StartCoroutine(AILoop());
        }
        else if (state == BossState.Dead)
        {
           OnDeath();
        }
    }

    bool IsInState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    void OnDeath()
    {
        OnDied?.Invoke();
        if (aiRoutine != null) StopCoroutine(aiRoutine);

        animator.ResetTrigger("hurt");
        animator.SetBool("Moving",false);

        state = BossState.Dead;
        animator.SetTrigger("Die");
        SoundFXManager.Instance.PlaySoundFXClip(deathSound,transform,deathSoundVolume);

        
    }

    public void ResetBossFight()
    {
        
        if (aiRoutine != null) StopCoroutine(aiRoutine);
        aiRoutine = null;

        
        HasFightBegun = false;
        state = BossState.Inactive;

        
        lastUse.Clear();

        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            
        }

        
        if (animator != null)
        {
            animator.ResetTrigger("Appear");
            animator.ResetTrigger("Hurt");
            animator.ResetTrigger("Die");
            animator.SetBool("Moving", false);

            
        }
    }

    public void die()
    {
        gameObject.SetActive(false);
    }
}
