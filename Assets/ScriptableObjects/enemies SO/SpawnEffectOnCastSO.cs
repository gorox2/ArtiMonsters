using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Boss/Attacks/Spawn Effect On Cast")]
public class SpawnEffectOnCastSO : BossAttackBase
{
    public enum SpawnMode
    {
        AtCastPoint,
        AtPlayerPosition,
        AtRandomArenaPoints,
        AttachedToBoss
    }

    [Header("Effect")]
    public GameObject effectPrefab;
    public SpawnMode spawnMode = SpawnMode.AtCastPoint;

    [Header("Count / Pattern")]
    [Min(1)] public int spawnCount = 1;
    public float randomRadius = 3f;              
    public float spreadAngleDeg = 0f;            
    public bool facePlayerBeforeCasting = true;

    [Header("Offsets")]
    public Vector2 playerSpawnOffset = new Vector2(0f, 3.5f);

    [Header("Random arena points")]
    public Transform arenaCenter;                
    public Vector2 arenaHalfExtents = new Vector2(6f, 3f); 
    public LayerMask groundMask;                 

   

    private void OnEnable() => attackType = AttackType.Cast;

    protected override IEnumerator DoExecute(BossController boss)
    {
        if (effectPrefab == null) yield break;

        bool fired = false;
        bool ended = false;

        void OnFire() => fired = true;
        void OnEnd() => ended = true;

        boss.AnimEvents.CastFire += OnFire;
        boss.AnimEvents.CastEnd += OnEnd;

        if (facePlayerBeforeCasting && boss.Player != null)
        {
            // face player right before casting
            var facing = boss.GetComponent<BossFacing>();
            if (facing != null) facing.FaceTarget(boss.Player.position);
        }

        boss.Animator.SetTrigger("Cast");

        
        yield return new WaitUntil(() => fired);

        SpawnEffects(boss);

        
        yield return new WaitUntil(() => ended);

        boss.AnimEvents.CastFire -= OnFire;
        boss.AnimEvents.CastEnd -= OnEnd;
    }

    void SpawnEffects(BossController boss)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = GetSpawnPosition(boss);
            Quaternion rot = Quaternion.identity;

            // if you're spawning projectiles and want spread
            if (spreadAngleDeg != 0f && boss.Player != null)
            {
                Vector2 dir = (boss.Player.position - pos).normalized;
                float angleOffset = (spawnCount == 1) ? 0f : Mathf.Lerp(-spreadAngleDeg * 0.5f, spreadAngleDeg * 0.5f, i / (float)(spawnCount - 1));
                float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                rot = Quaternion.Euler(0, 0, baseAngle + angleOffset);
            }

            GameObject go = Object.Instantiate(effectPrefab, pos, rot);

            // If attached, parent it
            if (spawnMode == SpawnMode.AttachedToBoss)
            {
                go.transform.SetParent(boss.transform, worldPositionStays: true);
            }

            
            var ctx = go.GetComponent<SpawnedByBoss>();
            if (ctx != null) ctx.Init(boss);
        }
    }

    Vector3 GetSpawnPosition(BossController boss)
    {
        switch (spawnMode)
        {
            case SpawnMode.AtCastPoint:
                return boss.CastPoint != null ? boss.CastPoint.position : boss.transform.position;

            case SpawnMode.AtPlayerPosition:
                if (boss.Player != null)
                    return boss.Player.position + (Vector3)playerSpawnOffset;
                return boss.transform.position;

            case SpawnMode.AttachedToBoss:
                return boss.transform.position;

            case SpawnMode.AtRandomArenaPoints:
            default:
                {
                    Vector3 center = arenaCenter != null ? arenaCenter.position : boss.transform.position;

                    // pick random point 
                    float x = Random.Range(center.x - arenaHalfExtents.x, center.x + arenaHalfExtents.x);
                    float y = Random.Range(center.y - arenaHalfExtents.y, center.y + arenaHalfExtents.y);
                    Vector3 p = new Vector3(x, y, 0);

                    // drop to ground
                    if (groundMask.value != 0)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(p + Vector3.up * 10f, Vector2.down, 30f, groundMask);
                        if (hit.collider != null) p = hit.point;
                    }

                    return p;
                }
        }
    }
}
