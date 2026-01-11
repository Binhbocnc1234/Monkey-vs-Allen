public class Thorn : IEffect, IOnDamageTaken {
    public Thorn(IEntity owner, int duration, int reflectPercent) : base(owner, duration, reflectPercent) {
        
    }
    public void OnDamageTaken(DamageContext ctx) {
        if(ctx.isMagicalDamage == false && ctx.attacker != null) {
            ctx.attacker.TakeDamage(new DamageContext(ctx.amount * strength / 100, null, owner, true));
        }
    }
}