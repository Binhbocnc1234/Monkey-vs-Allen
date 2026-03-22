public class IronBody : Effect, IDamageInputModifier {
    public IronBody(float duration, int strength) : base(duration, strength) {

    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.amount -= strength;
    }
}