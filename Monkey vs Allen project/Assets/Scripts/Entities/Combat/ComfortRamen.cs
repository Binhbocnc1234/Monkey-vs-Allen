using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComfortRamen : AimBullet
{
    public float rotateSpeed;
    public int healingPercent;
    public void Initialize(IEntity owner, IEntity target, int healingPercent){
        Initialize(0, owner, target);
        this.healingPercent = healingPercent;
    }
    protected override void Update() {
        base.Update();
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
    protected override void OnHit(IEntity target) {
        target.Heal(target[ST.MaxHealth] / 2);
        Destroy(this.gameObject);
    }
}
