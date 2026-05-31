public class YornInfinityArrow : RangedAttack {
    public enum State { Normal, Enhanced }
    public float maxHealthPercent = 5;
    public State state;
    private int shotIndex;
    private Timer infinityArrowsTimer;
    public override void Initialize() {
        base.Initialize();
        state = State.Normal;
        infinityArrowsTimer = new Timer(2, reset: true);
    }
    public override void UpdateBehaviour(float deltaTime) {
        if(state == State.Enhanced) {
            if(infinityArrowsTimer.Count(deltaTime)) {
                shotIndex++;
                MakeDamageByInfinityArrow();
            }
            if (shotIndex == 5) {
                state = State.Normal;
                shotIndex = 0;
            }
        }
        else if (state == State.Normal){
            base.UpdateBehaviour(deltaTime);
        }
    }
    protected override void WhenAttackReady() {
        base.WhenAttackReady();
        shotIndex++;
        if (shotIndex == 5) {
            state = State.Enhanced;
        }
    }
    void MakeDamageByInfinityArrow() {
        foreach(IEntity target in EContainer.Ins.GetEntitiesByLane(e.lane)) {
            if(e.DistanceTo(target) <= e[ST.Range]) {
                target.TakeDamage(new DamageContext(target.Stats[ST.MaxHealth] * maxHealthPercent / 100f, e, target, false));
            }
        }
    }
    public override string GetAnimatorStateName() {
        return state == State.Normal ? base.GetAnimatorStateName() : "Skill 1";
    }
}