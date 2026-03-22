using UnityEngine;
using System.Collections.Generic;
using System;
public abstract class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    public Model model;
    public float speed = 10f;
    [ReadOnly] public float damage = 10;
    [ReadOnly] public int lane = 0;
    public Team team;
    public IEntity owner;
    protected virtual void Initialize(float damage, IEntity owner) {
        this.owner = owner;
        this.damage = damage;
        this.lane = owner.lane;
        this.team = owner.team;
    }
    protected virtual void OnHit(IEntity target)
    {
        target.TakeDamage(new DamageContext(damage, owner, target, false));
        DestroyBullet();
    }

    protected virtual void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
