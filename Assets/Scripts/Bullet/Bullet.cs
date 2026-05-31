using UnityEngine;

public abstract class Bullet : MonoBehaviour, IBullet
{
    [Header("Bullet Properties")]
    public Model model;
    public float speed = 10f;
    [ReadOnly] public float damage = 10;
    [ReadOnly] public int lane = 0;
    public Team team;
    public IEntity owner;

    public virtual void Initialize(BulletSpawnRequest request) {
        owner = request.owner;
        damage = request.damage;
        lane = request.lane;
        team = request.owner.team;
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
