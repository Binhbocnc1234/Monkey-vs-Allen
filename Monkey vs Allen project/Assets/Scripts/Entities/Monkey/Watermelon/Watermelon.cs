using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watermelon : CollisionBullet
{
    private float upwardVec;
    public void Initialize(float speed, int damage, Entity owner, float upwardVec) {
        base.Initialize(speed, damage, owner);
        this.upwardVec = upwardVec;
    }
    protected override void Update() {
        base.Update();
        transform.position += Vector3.up * upwardVec * Time.deltaTime;
        upwardVec -= TechnicalInfo.gravity * Time.deltaTime;
    }
    
}
