using System;
using UnityEngine;

public class BossDeathReset : MonoBehaviour, IDeathResettable
{
    public event Action onDeathReset;
    [SerializeField]private Transform startPos;
    private Quaternion startRot;
    private HealthScript health;

    private void Awake()
    {
        
        startRot = transform.rotation;
        health = GetComponent<HealthScript>();
    }

    public void ResetOnPlayerDeath()
    {
        // If boss is defeated, keep it dead
        if (LevelManager.Instance != null && LevelManager.Instance.BossDefeated)
        {
            Debug.Log("did not reset boss"); 
            return;
        }

       
        transform.position = startPos.position;
        transform.rotation = startRot;

        if (health != null)
        {
            health.currentHealth = health.maxHealth;
            health.HandleHealthBar();
        }

        
         onDeathReset?.Invoke();

        
        gameObject.SetActive(false);
    }
}
