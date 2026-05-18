public class Poisoning : Effect {
    private readonly Timer delayTimer;
    public Poisoning(IEntity owner) : base(-1) {
        delayTimer = new Timer(3, reset: true);
    }
    public override void Update() {
        base.Update();
        if(delayTimer.Count()) {
            owner.TakeDamage(new DamageContext(2, null, owner, true));
        }
    }

}