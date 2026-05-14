using System.Collections.Generic;
using UnityEngine;

// 
public abstract class Move : IBehaviour, IInterruptable {
    protected int GetNormalizedDirection() {
        return e.team == Team.Left ? 1 : -1;
    }
    public override void UpdateBehaviour(float deltaTime) {
        if(CheckNearbyAllies()) e.ReturnToIdleBehaviour();
    }
    public override List<APModifier> GetAssessPoint() {
        // Cần phải quyết định lại
        return new() { new APModifier(Operator.Addition, APType.Danger, e[ST.MoveSpeed] / 6) };
    }
    public override bool CanActive() {
        if(CheckNearbyAllies()) return false;
        foreach(IEntity otherE in EContainer.Ins.GetEntitiesByLane(e.lane)) {
            if(otherE == e) continue;
            // Phía trước có 2 đồng minh thì dừng lại
            // Gặp kẻ địch thì dừng lại
            if(otherE.team != e.team && otherE.DistanceTo(e) <= e[ST.Range]) {
                return false;
            }
        }

        return true;
    }
    /// <summary>
    /// Nếu có từ 2 đồng minh trở lên đang ở trước mặt, trả về true
    /// </summary>
    /// <returns></returns>
    bool CheckNearbyAllies() {
        int countNearbyAllies = 0;
        foreach(IEntity otherE in EContainer.Ins.GetEntitiesByLane(e.lane)) {
            if(otherE == e || otherE.GetSO().AnyTribes(new() { Tribe.Target, Tribe.Tower })) continue;
            // Phía trước có 2 đồng minh thì dừng lại
            if(otherE.team == e.team && e.DistanceToBase() < otherE.DistanceToBase() && e.DistanceTo(otherE) <= 1f) {
                countNearbyAllies++;
                if(countNearbyAllies >= 2) {
                    return true;
                }
            }
        }
        return false;
    }
    public static float GetUnityMoveSpeed(IEntity e) => e.GetRealMoveSpeed() * IGrid.CELL_SIZE;
    public override int GetPriority() => 1;
    public override string GetAnimatorStateName() => "Walk"; 
}