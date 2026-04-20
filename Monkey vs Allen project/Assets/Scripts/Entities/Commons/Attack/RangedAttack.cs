using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Attack {
    [Header("Range fields")]
    public StraightBullet bulletPrefab;
    public Transform firePoint;
    protected Entity target;
    protected override void MakeDamageInstantly() {
        StraightBullet newBullet = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity);
        Debug.Log($"Bullet spawned at {newBullet.transform.position}");
        newBullet.Initialize(e[ST.Strength], e);
    }
}
