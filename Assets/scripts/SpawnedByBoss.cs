using UnityEngine;

public class SpawnedByBoss : MonoBehaviour
{
    public BossController Boss { get; private set; }

    public void Init(BossController boss)
    {
        Boss = boss;
    }
}
