using UnityEngine;

public class ComfortRamen : AimBullet
{
    public float rotateSpeed;
    public int healingPercent;

    public override void Initialize(BulletSpawnRequest request)
    {
        base.Initialize(request);
        healingPercent = 50; // heals 50% of max health
    }

    protected override void Update() {
        base.Update();
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
    protected override void OnHit(IEntity target) {
        target.Heal(target[ST.MaxHealth] / 2);
        Destroy(this.gameObject);
    }
}
