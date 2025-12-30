using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public float health;

    public Slider healthBar;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    public void TakeHit(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        updateHealthBar();
    }

   private void updateHealthBar()
    {
        
        healthBar.value = health;
        
    }
}
