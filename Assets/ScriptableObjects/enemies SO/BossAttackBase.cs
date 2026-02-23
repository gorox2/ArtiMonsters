using UnityEngine;
using System.Collections;


public abstract class BossAttackBase : ScriptableObject
{
    public enum AttackType { Melee, Cast, Other }
    public AttackType attackType = AttackType.Melee;

    [Header("Selection")]
    public float weight = 1f;
    public float cooldown = 2f;
    public float minRange = 0f;
    public float maxRange = 2.5f;

    float lastUseTime = -999f;

    public bool IsOffCooldown => Time.time >= lastUseTime + cooldown;

    public bool CanUse(BossController boss)
    {
        if (boss.Player == null) return false;
        if (!boss.IsAttackOffCooldown(this)) return false;

        float dist = Vector2.Distance(boss.transform.position, boss.Player.position);
        if (dist < minRange || dist > maxRange) return false;

        return AdditionalCanUse(boss);
    }

    protected virtual bool AdditionalCanUse(BossController boss) => true;

    public IEnumerator Execute(BossController boss)
    {
        boss.MarkAttackUsed(this);
        yield return DoExecute(boss);
    }

    protected abstract IEnumerator DoExecute(BossController boss);
}
