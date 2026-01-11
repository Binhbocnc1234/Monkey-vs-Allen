using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatermelonRangedAttack : RangedAttack
{
    protected override void MakeDamageInstantly() {
        Entity prey = GetNearestPrey();
        Watermelon watermelon = Instantiate(bulletPrefab.GetComponent<Watermelon>(), firePoint.position, Quaternion.identity);
        watermelon.Initialize(bulletSpeed, damage, e, CalculateUpwardVec(firePoint.position, prey.transform.position, bulletSpeed, e.GetComponent<Move>().moveSpeed));
    }
    float CalculateUpwardVec(Vector2 start, Vector2 target, float bulletVecX, float targetVecX){
        float dx = target.x - start.x;
        float dv = bulletVecX - targetVecX;
        if (dv == 0) return float.NaN;

        float t = dx / dv;
        if (t <= 0) return float.NaN;

        const float g = 10f;
        float dy = target.y - start.y;
        
        return (dy / t) + 0.5f * g * t;
    }


}
