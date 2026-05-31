using System.Collections.Generic;
using System.Linq;

public class ComfortRamenSkill : Skill
{
    // [Wrapper] Phase 4: inject via wrapper
    public float skillDurationLength;
    private IEntity bestCandidate;
    public override void Initialize() {
        base.Initialize();
        skillDuration = new Timer(skillDurationLength, false);
    }
    public override void UpdateBehaviour(float deltaTime) {
        if(skillDuration.Count(deltaTime)) {
            OnAnimationFinished();
        }
    }
    public override bool CanActive() {
        List<IEntity> candidates = new();
        foreach(IEntity otherE in IEntityRegistry.Ins.GetEntitiesByLane(e.lane)) {
            if(otherE != e && otherE.team == e.team && e.DistanceTo(otherE) < e.GetSkillStat(so, "Range")
                && e.GetHealthPercentage() <= e.GetSkillStat(so, "HealingPercent")/2) {
                candidates.Add(otherE);
            }
        }
        // Không có bất cứ con khỉ thỏa mãn 
        if (candidates == null){ return false; }
        bestCandidate = candidates[0];
        float lowestHealthPercent = 100;
        foreach(IEntity can in candidates) {
            if (can.GetHealthPercentage() <= lowestHealthPercent) {
                bestCandidate = can;
                lowestHealthPercent = can.GetHealthPercentage();
            }
        }
        return cooldownTimer.isEnd;
    }
    // [Wrapper] Phase 4: OnAnimationFinished should call wrapper to Instantiate visual
    public void OnAnimationFinished() {
        e.ReturnToIdleBehaviour();
        bestCandidate.Heal(30);
    }

    public override List<APModifier> GetAssessPoint() {
        return new(){new APModifier(Operator.Addition, APType.Buff, 100)};
    }
}
