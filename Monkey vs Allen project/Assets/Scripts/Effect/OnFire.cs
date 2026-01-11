public class OnFire : IEffect {
    private readonly Timer delayTimer;
    public OnFire(IEntity owner) : base(owner, 3) {
        isDebuff = true;
        delayTimer = new Timer(1, true);
    }
    public override void Update() {
        base.Update();
        if(delayTimer.Count()) {
            owner.TakeDamage(new DamageContext(3, null, owner, false));
        }
    }
}