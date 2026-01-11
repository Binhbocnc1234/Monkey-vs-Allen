using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreciseRangedAttack : Attack {
    [Header("Range fields")]
    public int bulletSpeed = 1;
    public AimBullet bulletPrefab;
    public Transform firePoint;
    protected Entity target;
    public override void Initialize() {
        base.Initialize();
        bulletPrefab.gameObject.SetActive(false);
    }
    protected override void MakeDamageInstantly() {
        if(target == null || target.IsDead() || target.Distance(e) > attackRange) {
            target = GetNearestPrey();
        }
        AimBullet newBullet = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity, this.transform);
        newBullet.Initialize(bulletSpeed, damage, e, target);
    }

}
