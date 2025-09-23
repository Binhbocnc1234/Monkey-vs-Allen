using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//When attacking, entities cannot move and otherwise!
public class RangedAttack : Attack
{
    [Header("Range fields")]
    public int bulletSpeed = 1;
    public AimBullet bulletPrefab;
    public Transform firePoint;
    Entity target;
    public override void Initialize() {
        base.Initialize();
        bulletPrefab.gameObject.SetActive(false);
    }
    protected override void MakeDamageInstantly() {
        if (target == null || target.IsDead()){
            SetNearestTarget();
        }
        AimBullet newBullet = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity, this.transform);
        newBullet.Initialize(bulletSpeed, damage, e.team, target);
    }
    bool SetNearestTarget(){
        var preys = GetPreyNearby();
        if(preys.Count == 0) { return false; }
        foreach(Entity prey in preys){
            if ((prey.GetWorldPosition().x - e.GetWorldPosition().x) <= attackRange){
                target = prey;
            }
        }
        return true;
    }
}
