using System;
using UnityEngine;

public class Poke : CollisionBullet
{
    [NonSerialized] public IEntity nearestTarget;
    private float stunDuration;
    private bool isDebuffApplied;
    public override void Initialize(BulletSpawnRequest request)
    {
        base.Initialize(request);
        this.isDebuffApplied = false;
        this.stunDuration = 2f;
    }

    public void SetStunParams(float duration, bool debuff) {
        stunDuration = duration;
        isDebuffApplied = debuff;
    }

    protected override void Update() {
        base.Update();
    }
    protected override void OnHit(IEntity target) {
        base.OnHit(target);
        target.GetEffectable().ApplyEffect(new Stun(stunDuration));
        if(isDebuffApplied) {
            target.GetEffectable().ApplyEffect(new SlimzSting(stunDuration));
        }
    }
}
