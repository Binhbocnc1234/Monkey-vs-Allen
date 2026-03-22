public class YornInfinityArrow : PreciseRangedAttack {
    public enum State { Normal, Enhanced }
    public float maxHealthPercent = 5;
    public State state;
    private int shotIndex;
    private Timer infinityArrowsTimer;
    public override void Initialize() {
        base.Initialize();
        infinityArrowsTimer = new Timer(2, reset: true);
    }
    public override void UpdateBehaviour() {
        if(state == State.Enhanced) {
            if(infinityArrowsTimer.Count()) {
                shotIndex++;
                MakeDamageByInfinityArrow();
            }
            if (shotIndex == 5) {
                state = State.Normal;
                shotIndex = 0;
            }
        }
        else if (state == State.Normal){
            base.UpdateBehaviour();
        }
    }
    protected override void MakeDamageInstantly() {
        base.MakeDamageInstantly();
        shotIndex++;
        if (shotIndex == 5) {
            state = State.Enhanced;
            e.model.PlayAnimation("InfinityArrow");
        }
    }
    void MakeDamageByInfinityArrow() {
        foreach(IEntity entity in EContainer.Ins.GetEntitiesByLane(e.lane)) {
            if (e.DistanceTo(entity) <= e[ST.Range]) {
                target.TakeDamage(new DamageContext(entity.Stats[ST.MaxHealth] * maxHealthPercent / 100f, e, entity, false));
            }
        }
    }
}