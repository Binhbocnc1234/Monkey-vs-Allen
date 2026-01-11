
using System;

public class ArmorBreak : IEffect, IDamageOutputModifier, IStackable {
    int count = 0;
    public ArmorBreak(IEntity owner) : base(owner, 5) {

    }
    public void ModifyDamage(DamageContext ctx) {
        ctx.penetrationAmount += count * 4;
    }
    public void Stack(int amount) {
        count = Math.Min(count + amount, 4);
    }
}