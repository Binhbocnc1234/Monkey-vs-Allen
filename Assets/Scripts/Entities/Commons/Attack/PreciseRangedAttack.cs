using UnityEngine;

[System.Serializable]
public class PreciseRangedAttack : RangedAttack {
    protected IEntity target;
    protected override void WhenAttackReady() {
        if(target == null || target.IsDead() || target.DistanceTo(e) > e[ST.Range]) {
            target = GetNearestPreyInRange();
        }
        if(target == null || bulletPrefab == null) return;

        var request = new BulletSpawnRequest {
            prefab = bulletPrefab,
            position = firePoint.position,
            rotation = Quaternion.identity,
            owner = e,
            target = target,
            damage = e[ST.Strength],
            lane = e.lane
        };
        IBulletSpawner.Ins.Spawn(request);
    }
}
