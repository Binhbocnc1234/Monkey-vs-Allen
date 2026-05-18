public class Deadly : Effect, IDamageOutputModifier {
    public Deadly(int duration = -1) : base(duration) {
        
    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.defender.GetEffectable().ApplyEffect(new DeadlyMark());
    }

    public override int GetDangerPoint() {
        int total = 0;
        return total;
    }
}