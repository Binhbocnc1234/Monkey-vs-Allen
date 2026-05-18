public class IronBody : Effect, IDamageInputModifier {
    public IronBody(float duration, int strength) : base(duration, strength) {

    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.AddModifier(new DamageModifier(Operator.Addition, -strength));
    }
}