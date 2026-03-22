using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Attack : IBehaviour, IOnApply
{
    /// <summary>
    /// Chỉ định thời gian để hành động tấn công kết thúc, không ảnh hưởng tới thời gian của animation
    /// </summary>
    public const float speedMultiplier = 1.35f;
    [SerializeField] protected Timer attackTimer;
    [SerializeField] protected Timer animationTimer;
    protected Entity defender;
    public override void Initialize() {
        base.Initialize();
        e = GetComponent<Entity>();
        var so = e.GetSO();
        attackTimer = new Timer(1 / e[ST.AttackSpeed] * speedMultiplier, false);
        animationTimer = new Timer(1 / e[ST.AttackSpeed] * speedMultiplier, false);
        e.GetAnimatorEvent().OnMakeDamage += MakeDamageInstantly;
    }
    void Update() {
        attackTimer.Count();
    }
    public override void UpdateBehaviour() {
        if(animationTimer.Count()) {
            e.ReturnToIdleBehaviour();
        }
    }
    public override bool CanActive() {
        if(!attackTimer.isEnd) return false;
        if (defender == null || e.DistanceTo(defender) > e[ST.Range] + 0.1f) {
            defender = GetNearestPreyInRange();
        }
        return defender != null;
    }
    public void OnApply() {
        attackTimer.totalTime = 1 / e[ST.AttackSpeed] * speedMultiplier;
        animationTimer.totalTime = attackTimer.totalTime;
        attackTimer.Reset();
        animationTimer.Reset();
        e.model.PlayAnimation("Attack");
        e.GetAnimator().SetFloat("AttackSpeed", e[ST.AttackSpeed] / e.GetSO().attackSpeed);
    }
    protected virtual void MakeDamageInstantly(){
        if (e == null || e.IsDead() || defender == null || defender.IsDead()){ return; }
        defender.TakeDamage(new DamageContext(e[ST.Strength], e, defender));
    }
    protected Entity GetNearestPreyInRange() {
        var container = EContainer.Ins.GetEntitiesByLane(e.lane);
        if (container.Length == 1) {
            // Vì chỉ có mỗi bản thân entity ở lane đó nên không có địch thủ
            return null;
        }
        IEntity result = null;
        foreach(IEntity otherE in container) {
            if(otherE == e || otherE.team == e.team) { continue; }
            if(result == null) {
                result = otherE;
            }
            if(otherE.DistanceTo(e) < result.DistanceTo(e)) {
                result = otherE;
            }
        }
        if (result == null){ return null; }
        if (result.DistanceTo(e) > e[ST.Range]) {
            return null;
        }
        return result.GetComponent<Entity>();
    }
    public override int GetPriority() => 2;
}
