using UnityEngine;

public class BODSpell : MonoBehaviour
{
    [SerializeField] GameObject spellPref;

    [Header("Damage")]
    [SerializeField] float damage = 25f;
    [SerializeField] LayerMask targetLayer;

    [SerializeField] Vector2 hitBoxSize = new Vector2(2.0f, 1.0f);
    [SerializeField] Vector2 hitBoxOffset = new Vector2(0f, -3.4f);

    [SerializeField] float autoDestroyAfter = 6f;

    Transform player;
    bool impactDone;
    
    void Start()
    {
        Destroy(gameObject, autoDestroyAfter);
    }


    public void Init(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void AE_Impact()
    {
        if (impactDone) return;
        impactDone = true;

        Vector2 center = (Vector2)transform.position + hitBoxOffset;

        Collider2D hit = Physics2D.OverlapBox(center, hitBoxSize, 0f, targetLayer);
        if (hit != null)
        {
            var hp = hit.GetComponent<HealthScript>();
            if (hp != null) hp.takeDamage(damage);
        }
    }

    public void AE_Finished()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Vector2 center = (Vector2)transform.position + hitBoxOffset;
        Gizmos.DrawWireCube(center, hitBoxSize);
    }
}
