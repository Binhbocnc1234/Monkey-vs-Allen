// [Wrapper] Phase 4: firePoint, bigWatermelon are Unity visual references
public class WatermelonStrike : Skill {
    IEntity lowestHealthEntity = null;
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
    }
}