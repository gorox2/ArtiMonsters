using Unity.VisualScripting;
using UnityEngine;

public class EnemyDeathReset : MonoBehaviour, IDeathResettable
{
    private Vector3 startPos;
    private Quaternion startRot;
    private HealthScript health;
    private smallImp enemyScript;

    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        health = GetComponent<HealthScript>();
        enemyScript = GetComponent<smallImp>();
    }

    public void ResetOnPlayerDeath()
    {
        gameObject.SetActive(true);

        transform.position = startPos;
        transform.rotation = startRot;

        if (health != null)
        {
            health.currentHealth = health.maxHealth;
            health.HandleHealthBar();
        }

        enemyScript.resetEnemy();
        
    }
}
