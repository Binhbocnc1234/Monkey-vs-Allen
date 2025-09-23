using UnityEngine;
using System.Collections.Generic;

public class AreaOfEffect : MonoBehaviour
{
    public float radius = 3f;
    public int damage = 20;
    public float duration = 2f;
    public Team team;

    void Start()
    {
        // Apply damage to all enemies in radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        
        foreach (Collider2D collider in colliders)
        {
            Entity entity = collider.GetComponent<Entity>();
            if (entity != null && entity.team != team)
            {
                entity.TakeDamage(damage);
            }
        }
        
        // Destroy after duration
        Destroy(gameObject, duration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

