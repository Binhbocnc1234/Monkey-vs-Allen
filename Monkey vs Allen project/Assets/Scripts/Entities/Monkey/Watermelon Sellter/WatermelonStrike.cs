using UnityEngine;

public class WatermelonStrike : Skill {
    IEntity lowestHealthEntity = null;
    public Transform firePoint;
    public GameObject bigWatermelon;
    public override bool CanActive() {
        lowestHealthEntity = null;
        foreach(IEntity otherE in IEntityRegistry.Ins.GetEntities()) {
            if(e.DistanceTo(otherE) > e[ST.Range] && e.team == otherE.team) continue;
            if(lowestHealthEntity == null) lowestHealthEntity = otherE;
            else if(otherE[ST.Health] < lowestHealthEntity[ST.Health]) {
                lowestHealthEntity = otherE;
            }
        }
        if(lowestHealthEntity != null) { return true; }
        return cooldownTimer.isEnd;
    }
    public override void Initialize() {
        base.Initialize();
        e.model.Event.OnAnimationFinished += (info) => {
            if (info.IsName($"Skill {skillIndex}")) {
                // Create Watermelon bullet
                GeneralPurposeContainer.Ins.CreateInstance(bigWatermelon, firePoint.position);
                
            }
        };
    }
}