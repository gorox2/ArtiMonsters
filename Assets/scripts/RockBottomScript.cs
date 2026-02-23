using UnityEngine;

public class RockBottomScript : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<HealthScript>())
        {
            HealthScript healthScript = collision.gameObject.GetComponent<HealthScript>();
            float damage = healthScript.maxHealth;
            healthScript.takeDamage(damage);
        }
    }
}
