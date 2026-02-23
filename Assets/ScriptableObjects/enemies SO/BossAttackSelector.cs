using UnityEngine;

public static class BossAttackSelector 
{
    public static BossAttackBase Choose(BossAttackBase[] attacks, BossController boss)
    {
        float total = 0f;

        if (attacks == null || attacks.Length == 0) return null;
        int usableCount = 0;

        for (int i = 0; i < attacks.Length; i++)
        {
            var a = attacks[i];
            if (a == null) continue;

            if (a.CanUse(boss))
            {
                float w = Mathf.Max(0f, a.weight);
                if (w > 0f)
                {
                    total += w;
                    usableCount++;
                }
            }
        }

        if (total <= 0f || usableCount == 0) return null;

        float roll = Random.value * total;
        float cumulative = 0f;

        for (int i = 0; i < attacks.Length; i++)
        {
            var a = attacks[i];
            if (a == null) continue;

            if (!a.CanUse(boss)) continue; 
            float w = Mathf.Max(0f, a.weight);
            if (w <= 0f) continue;

            cumulative += w;
            if (roll <= cumulative) return a;
        }

        return null;
    }
}
