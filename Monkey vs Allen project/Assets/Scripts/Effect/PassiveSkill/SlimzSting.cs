public class SlimzSting : Effect, IDamageOutputModifier {
    public SlimzSting(float duration) : base(duration) {

    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.damageMultiplier *= 1.25f;
    }
}