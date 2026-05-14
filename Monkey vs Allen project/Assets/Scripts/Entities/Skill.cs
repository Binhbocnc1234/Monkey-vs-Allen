using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.IO.LowLevel.Unsafe;
/// <summary>
/// Skill is owned by only one type of Entity and its signature of that Enttiy <br/>
/// Skill is executed after timer is end
/// </summary>
[RequireComponent(typeof(Entity))]
public abstract class Skill : IBehaviour, IOnApply, IInitialize{
    protected float cooldown;
    public SkillSO so;
    public int skillIndex = 1;
    protected Timer cooldownTimer;
    public Timer skillDuration;
    public StunImmunity stunImmunity;
    public virtual void Initialize() {
        cooldownTimer = new Timer(e.GetSkillStat(so, "Cooldown"), reset: false);
        // e.model.Event.OnAnimationFinished += (info) => {
        //     if (info.IsName($"Skill {skillIndex}")) {
        //         e.GetEffectable().RemoveEffect(stunImmunity);
        //         e.ReturnToIdleBehaviour();
        //     }
        // };
        stunImmunity = new StunImmunity();
    }
    void Update() {
        cooldownTimer.Count();
    }
    public virtual void OnApply() {
        cooldownTimer.Reset();
        e.GetEffectable().ApplyEffect(stunImmunity);
    }
    public override string GetAnimatorStateName() => $"Skill {skillIndex}";
    public override int GetPriority() => 4;
}
