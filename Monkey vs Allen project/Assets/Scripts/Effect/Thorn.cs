public class Thorn : Effect, IOnDamageTaken {
    public Thorn(int duration, int reflectPercent) : base(duration, reflectPercent) {
        
    }
    public void OnDamageTaken(DamageContext ctx) {
        if(ctx.isMagicalDamage == false && ctx.attacker != null) {
            ctx.attacker.TakeDamage(new DamageContext(ctx.amount * strength / 100, null, owner, true));
        }
    }
}