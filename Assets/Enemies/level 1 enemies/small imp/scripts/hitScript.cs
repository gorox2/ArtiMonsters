using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitScript : MonoBehaviour
{
    public Animator anim;
    public Vector2 hitBoxSize;
    public LayerMask playerLayer;


    public void dealDamageToPlayer(float damage)
    {
        Collider2D[] playerhit = Physics2D.OverlapBoxAll(transform.position, hitBoxSize, 0, playerLayer);
        foreach (Collider2D player in playerhit)
        {
            if (player.CompareTag("Player"))
            {
                GameObject playerGO = player.gameObject;

                PlayerHealth playerHealth = playerGO.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeHit(damage);
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, hitBoxSize);
    }

}
