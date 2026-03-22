using UnityEngine;

// 
public abstract class Move : IBehaviour, IInterruptable, IOnApply {
    public virtual void OnApply() {
        e.model.PlayAnimation("Walk");
    }
    protected int GetNormalizedDirection() {
        return e.team == Team.Player ? 1 : -1;
    }
    public override void UpdateBehaviour() {
        
    }
    public override bool CanActive() {
        int countNearbyAllies = 0;
        foreach(IEntity otherE in EContainer.Ins.GetEntitiesByLane(e.lane)) {
            if(otherE == e) continue;
            // Phía trước có 2 đồng minh thì dừng lại
            if(otherE.team == e.team && e.DistanceToBase() < otherE.DistanceToBase() && e.DistanceTo(otherE) <= 0.5f) {
                countNearbyAllies++;
                if(countNearbyAllies == 3) {
                    return false;
                }
            }
            // Gặp kẻ địch thì dừng lại
            if(otherE.team != e.team && otherE.DistanceTo(e) <= e[ST.Range]) {
                return false;
            }
        }

        return true;
    }
    public override int GetPriority() => 1;
}