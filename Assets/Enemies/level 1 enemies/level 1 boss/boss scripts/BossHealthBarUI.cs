using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] TMP_Text bossNameText;
    [SerializeField] Image fill;

    HealthScript healthScript;

    private void Awake()
    {
        
        if (bossNameText == null) Debug.LogError($"{name}: bossNameText is not assigned.");
        if (fill == null) Debug.LogError($"{name}: fill Image is not assigned.");
    }

    

    private void OnDisable()
    {
        
        DisconnectFromHealth();
    }

    public void Show(BossController boss)
    {
        gameObject.SetActive(true);
        Debug.Log("Boss UI Show() called for: " + boss.BossName);

        bossNameText.text = boss.BossName;

        connectHealth(boss.Health);
    }

    public void Hide()
    {
        DisconnectFromHealth();
        gameObject.SetActive(false);
    }

    void connectHealth(HealthScript health)
    {
        DisconnectFromHealth();
        healthScript = health;

        healthScript.onHealthChanged += OnHealthChanged;
        healthScript.onDeath += OnDeath;

        // initialize immediately
        OnHealthChanged(healthScript.currentHealth, healthScript.maxHealth);
    }

    void DisconnectFromHealth()
    {
        if (healthScript == null) return;

        healthScript.onHealthChanged -= OnHealthChanged;
        healthScript.onDeath -= OnDeath;
        healthScript = null;
    }

    void OnHealthChanged(float current, float max)
    {
        fill.fillAmount = (max <= 0) ? 0 : current / max;
    }

    void OnDeath()
    {
        
        Hide();
    }
}
