using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    Animator anim;
    [SerializeField]
    Slider healthFill;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        healthFill = gameObject.GetComponentInChildren<Slider>();
        healthFill.maxValue = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        updateHealthbar(currentHealth,maxHealth);
        if (currentHealth <= 0) 
        {
            
            anim.SetTrigger("isdead");
            
        }
    }

    public void takeDamage(float damage)
    {
        anim.SetTrigger("ishit");
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f,maxHealth);
    }

    void updateHealthbar(float currenthealth , float maxhealth)
    {
        healthFill.value = currenthealth;
    }


    public void die()
    {

        Destroy(gameObject);
    }
}
