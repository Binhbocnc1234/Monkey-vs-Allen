using UnityEngine;
using System.Collections.Generic;
public abstract class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    [ReadOnly] public float speed = 10f;
    [ReadOnly] public int damage = 10;
    protected Vector3 direction;
    public Entity owner;
    protected virtual void Initialize(float speed, int damage, Entity owner)
    {
        this.owner = owner;
        this.speed = speed;
        this.damage = damage;
    }

    protected virtual void OnHit(Entity target)
    {
        if (target != null && target.team != owner.team)
        {
            DamageContext ctx = new DamageContext(damage, owner, target, false);
            owner.effectController.ProcessDamageOutput(ctx);
            target.TakeDamage(ctx);
            DestroyBullet();
        }
    }

    protected virtual void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
