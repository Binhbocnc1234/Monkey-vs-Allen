public class BullEye : Effect, IDamageOutputModifier {
    public BullEye(int duration) : base(duration) {

    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.penetrationAmount = 100;
    }
}