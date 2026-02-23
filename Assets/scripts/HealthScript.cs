using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthScript : MonoBehaviour
{
    public event Action<float> onDamaged;                 // damage amount
    public event Action<float> onHealed;                  // heal amount
    public event Action<float, float> onHealthChanged;    // current, max
    public event Action onDeath;
    public bool isDead = false;
    
    public float maxHealth;
    public float currentHealth;
    
    [SerializeField] Image healthBar;
    TMP_Text healthChangeText;
    GameObject healthChangeGO;
    
    
    private void OnEnable()
    {
        isDead = false;
        currentHealth = maxHealth;
        if (healthBar == null)
        {
            Debug.LogError("health bar missing");
        }
    }

    public void takeDamage(float damage)
    {
        if (isDead) return;
        if (damage < 0) return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        showHealthChange(-damage);
        HandleHealthBar();
        onDamaged?.Invoke(damage);
        EmitHealthChanged();
        if (currentHealth <= 0)
        {
            die();
        }
    }

    public void addHealth(float health)
    {
        if (isDead) return;
        if (health <= 0) return;
        currentHealth += health;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        showHealthChange(health);
        HandleHealthBar();
        onHealed?.Invoke(health);
        EmitHealthChanged();
        
    }

    public void showHealthChange(float amount)
    {
        Debug.Log(gameObject.name + "health's changed by" + amount);
        //healthChangeText.text = $"{amount}";
        //healthChangeGO.transform.Translate()
    }

    public void HandleHealthBar()
    { 
        if (!healthBar) return;
        healthBar.fillAmount = currentHealth/maxHealth;
    }

    public void increaseMaxHealth(float amount)
    {
        maxHealth += amount;

        if(maxHealth <= 0) maxHealth = 1;
        HandleHealthBar();
        EmitHealthChanged();
    }

    void EmitHealthChanged()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void die()
    {
        
        if (isDead) return;
        isDead = true;
        onDeath?.Invoke();
    }

}
