using System.Collections.Generic;

[System.Serializable]
/// <summary>
/// Base attack behaviour that handles timing, target selection, and the transition back to idle
/// </summary>
public abstract class AttackBase : IBehaviour, IOnApply, IInitialize, IUpdatePerFrame {
    public const float speedMultiplier = 1.35f;
    protected Timer attackTimer;
    protected Timer animationTimer;
    protected IEntity defender;
    public virtual void Initialize() {
        attackTimer = new Timer(1 / e[ST.AttackSpeed] * speedMultiplier, false);
        animationTimer = new Timer(1 / e[ST.AttackSpeed] * speedMultiplier, false);
    }
    public override void UpdateBehaviour(float deltaTime) {
        if(animationTimer.Count(deltaTime)) {
            if(e.isSimulated) {
                ApplyDirectDamage();
            }
            else {
                WhenAttackReady();
            }
            e.ReturnToIdleBehaviour();
        }
    }
    public void Update(float deltaTime){
        attackTimer.Count();
        animationTimer.Count();
    }
    protected virtual void ApplyDirectDamage() {
        if(e == null || e.IsDead() || defender == null || defender.IsDead()) { return; }
        defender.TakeDamage(new DamageContext(e[ST.Strength], e, defender));
    }
    public override bool CanActive() {
        if(!attackTimer.isEnd) return false;
        if(defender == null || defender.IsDead() || e.DistanceTo(defender) > e[ST.Range] + 0.1f) {
            defender = GetNearestPreyInRange();
        }
        return defender != null;
    }
    public void OnApply() {
        attackTimer.totalTime = 1 / e[ST.AttackSpeed] * speedMultiplier;
        animationTimer.totalTime = attackTimer.totalTime;
        attackTimer.Reset();
        animationTimer.Reset();
    }
    protected abstract void WhenAttackReady();
    protected IEntity GetNearestPreyInRange() {
        var container = EContainer.Ins.GetEntitiesByLane(e.lane);
        if(container.Length == 1) {
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
        if(result == null) { return null; }
        if(result.DistanceTo(e) > e[ST.Range]) {
            return null;
        }
        return result;
    }
    public override List<APModifier> GetAssessPoint() {
        return new() { new APModifier(Operator.Addition, APType.Danger,
            value: (0.9f + e[ST.Range] / 10) * e[ST.AttackSpeed] * e[ST.Strength])};
        // Ví dụ với Basic Alien thì giá trị này sẽ khoảng 30
    }
    public override int GetPriority() => 3;
    public override string GetAnimatorStateName() => "Attack";
}
