using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Attack {
    [Header("Range fields")]
    public StraightBullet bulletPrefab;
    public Transform firePoint;
    protected Entity target;
    public override void Initialize() {
        base.Initialize();
        bulletPrefab.gameObject.SetActive(false);
    }
    protected override void MakeDamageInstantly() {
        StraightBullet newBullet = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity);
        newBullet.Initialize(e[ST.Strength], e);
    }

}
