using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(menuName = "Boss/Attacks/Melee Box Attack")]
public class MeleeBoxAttackSO : BossAttackBase
{
    [Header("Melee")]
    public float damage = 20f;
    public Vector2 boxSize = new Vector2(1.5f, 1.0f);
    public float boxDistance = 1.0f;
    public LayerMask targetLayer;

    
    private void OnEnable()
    {
        attackType = AttackType.Melee;
    }

    protected override IEnumerator DoExecute(BossController boss)
    {
        bool hitFired = false;
        bool endFired = false;

        void OnHit() => hitFired = true;
        void OnEnd() => endFired = true;

        boss.AnimEvents.AttackHit += OnHit;
        boss.AnimEvents.AttackEnd += OnEnd;

        boss.Animator.SetTrigger("Melee");

       
        yield return new WaitUntil(() => hitFired);

        // Perform hit
        Vector2 origin = boss.hitPoint != null ? (Vector2)boss.hitPoint.position : (Vector2)boss.transform.position;
        Vector2 dir = boss.IsFacingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, dir, boxDistance, targetLayer);
        if (hit.collider != null)
        {
            var hp = hit.collider.GetComponent<HealthScript>();
            if (hp != null) hp.takeDamage(damage);
        }

        
        yield return new WaitUntil(() => endFired);

        boss.AnimEvents.AttackHit -= OnHit;
        boss.AnimEvents.AttackEnd -= OnEnd;
    }

    void OnDrawGizmosSelected()
    {
        
    }
}
