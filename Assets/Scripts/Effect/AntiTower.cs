public class AntiTower : Effect, IDamageOutputModifier {
    public AntiTower(int duration) : base(duration) {

    }
    public void ModifyDamage(DamageContext ctx) {
        if(ctx.defender.GetSO().tribes.Contains(Tribe.Tower)) {
            ctx.damageMultiplier *= 2;
        }
    }
}