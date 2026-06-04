using System;
using UnityEngine;

[System.Serializable]
public class RangedAttack : AttackBase {
    public GameObject bulletPrefab;
    [NonSerialized] public Transform firePoint;
    protected override void WhenAttackReady() {
        if(bulletPrefab == null) {
            Debug.LogError($"[RangedAttack] bulletPrefab is null");
            return;
        }
        if (firePoint == null) {
            Debug.LogError("[RangedAttack] firePoint is null");
        }

        var request = new BulletSpawnRequest {
            prefab = bulletPrefab,
            position = firePoint.position,
            rotation = Quaternion.identity,
            owner = e,
            target = null,
            damage = e[ST.Strength],
            lane = e.lane
        };
        IBulletSpawner.Ins.Spawn(request);
    }
}
