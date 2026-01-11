using System.Linq;

public class Armored : IEffect, IDamageInputModifier {
    public readonly int amount;
    public Armored(IEntity owner, int amount, int duration = -1) : base(owner, duration) {
        this.amount = amount;
    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.amount -= amount;
    }
}