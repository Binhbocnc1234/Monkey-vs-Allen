using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComfortRamenSkill : Skill
{
    public ComfortRamen bullet;
    public Transform firePoint;
    private IEntity bestCandidate;
    public override void Initialize() {
        base.Initialize();
        e.GetAnimatorEvent().OnAnimationFinished += (info) => {
            if (info.IsName("Skill 1")) {
                OnAnimationFinished();
            }
        };
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
        return cooldownTimer.isEnd; // Kiểm tra xem có kẻ địch nào ở gần trong tầm e[ST.Range] không
    }
    public void OnAnimationFinished() {
        e.ReturnToIdleBehaviour();
        ComfortRamen newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity, GeneralPurposeContainer.Ins.transform);
        newBullet.Initialize(e, bestCandidate, (int)e.GetSkillStat(so, "HealingPercent")/2);
    }
}
