using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    [SerializeField] BossFightManager boss;
   
    public bool triggered = false;
  
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        if (boss.BossDead)
        {
            Debug.Log("boss trigger returned");
            return;
        }

        triggered = true;
        
        boss.StartFight();

    }
}
