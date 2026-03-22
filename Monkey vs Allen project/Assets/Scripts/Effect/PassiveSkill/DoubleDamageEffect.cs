public class DoubleDamageEffect : GlobalEffect, IDamageOutputModifier {
    public void ModifyDamage(DamageContext ctx) {
        ctx.damageMultiplier *= 2;
    }
}