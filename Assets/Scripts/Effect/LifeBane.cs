public class LifeBane : Effect, IOnHeal {
    public LifeBane(float duration) : base(duration, 40) {

    }
    public void OnHeal(HealContext ctx) {
        ctx.amount /= 2;
    }
}