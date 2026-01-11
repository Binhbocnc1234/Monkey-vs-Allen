public class AntiTower : IEffect, IDamageOutputModifier {
    public AntiTower(IEntity owner, int duration) : base(owner, duration) {

    }
    public void ModifyDamage(DamageContext ctx) {
        if(ctx.defender.tribes.Contains(Tribe.Tower)) {
            ctx.amount *= 2;
        }
    }
}