using UnityEngine;
using System.Collections;

public class PlayerDeathRespawn : MonoBehaviour
{
    Animator animator;

    [Header("Refs")]
    [SerializeField] private HealthScript playerHealth;
    [SerializeField] private Transform startPoint; 

    [Header("disable control briefly")]
    [SerializeField] private MonoBehaviour[] disableDuringRespawn;
    [SerializeField] private float respawnDelay = 0.1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Reset()
    {
        playerHealth = GetComponent<HealthScript>();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.onDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.onDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // freeze input 
        foreach (var m in disableDuringRespawn)
            if (m) m.enabled = false;

        animator.SetTrigger("die");
        yield return new WaitForSeconds(respawnDelay);

        
        if (startPoint != null)
            transform.position = startPoint.position;
        
        playerHealth.isDead = false;
        playerHealth.currentHealth = playerHealth.maxHealth;
        playerHealth.HandleHealthBar();
        
        LevelResetSystem.ResetEnemiesAndBoss();

        // unfreeze input
        foreach (var m in disableDuringRespawn)
            if (m) m.enabled = true;
    }
}

