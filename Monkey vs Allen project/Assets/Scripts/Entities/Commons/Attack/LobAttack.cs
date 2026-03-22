using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobAttack : RangedAttack
{
    protected override void MakeDamageInstantly() {
        Entity prey = GetNearestPreyInRange();
        LobBullet watermelon = Instantiate(bulletPrefab.GetComponent<LobBullet>(), firePoint.position, Quaternion.identity);
        watermelon.Initialize(e[ST.Strength], e,
            CalculateUpwardVec(firePoint.position, prey.transform.position, watermelon.speed, e[ST.MoveSpeed]));
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
