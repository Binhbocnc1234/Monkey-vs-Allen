public class BullEye : IEffect, IDamageOutputModifier {
    public BullEye(IEntity owner, int duration) : base(owner, duration) {

    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.penetrationAmount = 100;
    }
}