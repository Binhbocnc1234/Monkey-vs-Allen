using UnityEngine;

[System.Serializable]
public class LobAttack : RangedAttack {
    protected override void WhenAttackReady() {
        IEntity prey = GetNearestPreyInRange();
        if(prey == null || bulletPrefab == null) return;

        var request = new BulletSpawnRequest {
            prefab = bulletPrefab,
            position = firePoint.position,
            rotation = Quaternion.identity,
            owner = e,
            target = prey,
            damage = e[ST.Strength],
            lane = e.lane
        };

        IBulletSpawner.Ins.Spawn(request);
    }
}
