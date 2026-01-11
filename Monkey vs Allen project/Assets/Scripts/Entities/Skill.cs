using System.Collections.Generic;
using System;
/// <summary>
/// Skill is owned by only one type of Entity and its signature of that Enttiy <br/>
/// Skill is executed after timer is end
/// </summary>
public abstract class Skill : IEffect{
    public float cooldown;
    public Timer cooldownTimer;
    public Skill(IEntity owner, float cooldown) : base(owner){
        this.cooldown = cooldown;
        cooldownTimer = new Timer(cooldown, reset: false);
    }
    public override void Update() {
        if(cooldownTimer.Count()) {
            cooldownTimer.Reset();
            ActiveSkill();
        }
        EntityState state = owner.GetEntityState();
        if(cooldownTimer.Count() && (state == EntityState.Idle || state == EntityState.Walk)) {
            owner.SetEntityState(EntityState.ActivateSkill);
        }
    }
    protected abstract bool CanActiveSkill();
    protected abstract void ActiveSkill();
}
