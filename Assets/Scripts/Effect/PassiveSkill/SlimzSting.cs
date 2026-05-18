public class SlimzSting : Effect, IDamageInputModifier {
    public SlimzSting(float duration) : base(duration) {

    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.AddModifier(new DamageModifier(Operator.Multiply, 1.25f));
    }
}