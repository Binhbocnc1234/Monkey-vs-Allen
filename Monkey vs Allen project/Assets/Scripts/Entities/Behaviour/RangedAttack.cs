using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Attack {
    [Header("Range fields")]
    [ReadOnly] public int bulletSpeed = 1;
    public CollisionBullet bulletPrefab;
    public Transform firePoint;
    protected Entity target;
    public override void Initialize() {
        base.Initialize();
        bulletPrefab.gameObject.SetActive(false);
    }
    protected override void MakeDamageInstantly() {
        CollisionBullet newBullet = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity);
        newBullet.Initialize(bulletSpeed, damage, e);
    }

}
