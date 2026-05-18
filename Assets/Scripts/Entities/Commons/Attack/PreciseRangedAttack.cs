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
        if(target == null || target.IsDead() || target.DistanceTo(e) > e[ST.Range]) {
            // Tìm target mới do target cũ đã không còn nằm trong vùng tấn công hoặc đã chết
            target = GetNearestPreyInRange();
        }
        AimBullet newBullet = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity, GeneralPurposeContainer.Ins.transform);
        newBullet.Initialize(e[ST.Strength], e, target);
    }

}
