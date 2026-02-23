using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.iOS;

public class PlayerAttack : MonoBehaviour
{
    Animator animator;
    playerMove playerMove;

    public Transform hitboxTransform;
    public Vector2 hitBoxSize;
    public LayerMask layer;

    public bool isAttacking;

    [SerializeField] float damage = 10f;
    [SerializeField] KeyCode attackKey = KeyCode.Mouse0;

    [Header("Audio")]
    [SerializeField] AudioClip attackSound;
    [SerializeField] float soundVolume = 1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }
    private void Start()
    {
        playerMove = gameObject.GetComponent<playerMove>();
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(attackKey) && !isAttacking && playerMove.isGrounded)
        {
            playerMove.canMove = false;
            Attack();
        }
    }

    void Attack()
    {
        getNextAttack();
        isAttacking = true;
        animator.Play("player_LA1");
        SoundFXManager.Instance.PlaySoundFXClip(attackSound,gameObject.transform, soundVolume);
        
    }

    void getNextAttack()
    {
        // we`ll write this when we get to the combo part
    }
    public void endAttack()
    {
        isAttacking = false;
        playerMove.canMove = true;
    }

    public void DealDamageToTarget()
    {
        Collider2D[] enemyhit = Physics2D.OverlapBoxAll(hitboxTransform.position, hitBoxSize, 0, layer);
        foreach (Collider2D enemy in enemyhit)
        {
            if (enemy.CompareTag("enemy"))
            {
                Debug.Log("enemy hit confirmed... damage: " + damage);
                HealthScript health = enemy.GetComponent<HealthScript>();
                if (health == null) { Debug.Log("opponent doesnt have healthScript"); continue; }
                health.takeDamage(damage);
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(hitboxTransform.position, hitBoxSize);
    } 
}




