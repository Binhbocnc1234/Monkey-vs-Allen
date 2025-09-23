using UnityEngine;
using System.Collections.Generic;
public class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    public float speed = 10f;
    public int damage = 10;
    public Team team;

    protected Vector3 direction;

    public virtual void Initialize(Vector3 direction, float speed, int damage, Team team)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        this.damage = damage;
        this.team = team;
    }

    protected virtual void OnHit(Entity target)
    {
        if (target != null && target.team != team)
        {
            target.TakeDamage(damage);
            DestroyBullet();
        }
    }

    protected virtual void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
