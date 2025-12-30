using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{

    public Transform atktrigger;
    public Animator anim;
    string animName;
    public Rigidbody2D playerrb;
    public Vector2 hitBoxSize;
    public LayerMask enemyLayers;
    bool eventTriggered = false;
    float damage;

    int attackCombo = 0;
    float comboTimer = 0;
    float comboMaxTime = 1f; 
    bool isAttacking = false;

    bool isChargingHeavyAttack = false;
    float heavyAttackChargeStartTime;
    public float maxChargeTime = 2f;
    public float maxDamage = 70f;



    // Update is called once per frame
    void Update()
    {
        #region light attack
        if (Input.GetKeyDown(KeyCode.Keypad4) && !isAttacking && playerMove.onGround)
        {
            attack();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4) && !isAttacking && !playerMove.onGround)
        {
            airAttack();
        }

        if (isAttacking)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboMaxTime)
            {
                attackCombo = 0;
                comboTimer = 0;
                isAttacking = false;
            }
        }
        #endregion

        #region Heavy attack
        if (isChargingHeavyAttack)
        {
            // Calculate charging duration
            float chargeDuration = Time.time - heavyAttackChargeStartTime;

            float damage = CalculateDamage(chargeDuration);


            if (chargeDuration >= maxChargeTime)
            {
                
                ExecuteHeavyAttack(damage);
                isChargingHeavyAttack=false;
            }
        }

        
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            isChargingHeavyAttack = true;
            heavyAttackChargeStartTime = Time.time;
            anim.SetTrigger("CHattack");
        }

        if (Input.GetKeyUp(KeyCode.Keypad6) && isChargingHeavyAttack)
        {
            // Calculate charging duration
            float chargeDuration = Time.time - heavyAttackChargeStartTime;

            float damage = CalculateDamage(chargeDuration);

            ExecuteHeavyAttack(damage);

            isChargingHeavyAttack = false;
        }
        #endregion

        

    }
    #region light attack
    void attack()
    {

        if (attackCombo == 0 || comboTimer > comboMaxTime)
        {
            attackCombo = 1;
        }
        else
        {
            attackCombo++;
        }

        comboTimer = 0; 
        

        switch (attackCombo)
        {
            case 1:
                
                damage = 20;
                anim.SetTrigger("LA1");
                animName = "player_LA1";
                DealDamageToTarget(damage);

                break;
            case 2:
                damage = 20;
                anim.SetTrigger("LA2");
                animName = "player_LA2";
                DealDamageToTarget(damage);

                break;
            case 3:
                damage = 25;
                anim.SetTrigger("LA3");
                animName = "player_LA3";
                DealDamageToTarget(damage);

                break;
            default:
                break;
        }

        attackCombo = Mathf.Clamp(attackCombo, 0, 3); 
        isAttacking = true;
    }
    #endregion

    #region jump attack
    void airAttack()
    {
        playerrb.gravityScale = 0;
        if (attackCombo == 0 || comboTimer > comboMaxTime)
        {
            attackCombo = 1;
        }
        else
        {
            attackCombo++;
        }

        comboTimer = 0;
        
        switch (attackCombo)
        {
            case 1:
                damage = 15f;
                anim.SetTrigger("JA1");
                animName = "player_JA1";
                DealDamageToTarget(damage);

                break;
            case 2:
                damage = 15f;
                anim.SetTrigger("JA2");
                animName = "player_JA2";
                DealDamageToTarget(damage);

                break;
            case 3:
                damage = 20f;
                anim.SetTrigger("JA3");
                animName = "player_JA3";
                DealDamageToTarget(damage);

                break;
            default:
                break;
        }
        if (playerMove.onGround) { attackCombo = 0; }

        attackCombo = Mathf.Clamp(attackCombo, 0, 3);
        isAttacking = true;

        playerrb.velocity = new Vector2(0, 0);

        
    }
    #endregion
    public void attackFinished()
    {
        playerrb.gravityScale = 1;

        isAttacking = false;
        if (comboTimer > comboMaxTime || attackCombo >= 3)
        {
            attackCombo = 0;
        }
    }

    #region Heavy attack
    float CalculateDamage(float chargeDuration)
    {
        damage = Mathf.Clamp((chargeDuration / maxChargeTime) * maxDamage, 0f, maxDamage);
        return damage;
    }

    void ExecuteHeavyAttack(float damage)
    {
       this.damage = damage;
        anim.SetTrigger("Hattack");

        animName = "player_HA";
        DealDamageToTarget(this.damage);



    }
    #endregion

    public void DealDamageToTarget(float damage)
    {
        Collider2D[] enemyhit = Physics2D.OverlapBoxAll(atktrigger.position, hitBoxSize, 0, enemyLayers);
        foreach(Collider2D enemy in enemyhit)
        {
            if (enemy.CompareTag("enemy"))
            {
                GameObject enemyGO = enemy.gameObject;

                HealthController healthControl = enemyGO.GetComponent<HealthController>();

                if(healthControl != null)
                {
                    healthControl.takeDamage(damage);
                }
                else { Debug.LogWarning("HealthController missing from enemy"); }

            }
        }

    }

    public void endAnimatin()
    {
        eventTriggered = false;
    }

    public bool attackHitRange()
    {
        Collider2D[] enemyhit = Physics2D.OverlapBoxAll(atktrigger.position, hitBoxSize, 0, enemyLayers);
        if(enemyhit.Length>0)
        {
            return true;
        }
        else return false;

        
    }

   

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(atktrigger.position, hitBoxSize);
    }
}
