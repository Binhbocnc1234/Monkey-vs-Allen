public class Deadly : IEffect, IDamageOutputModifier {
    public Deadly(IEntity owner, int duration = -1) : base(owner, duration) {
        
    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.defender.GetEffectController().ApplyEffect(new DeadlyMark(ctx.defender));
    }
}