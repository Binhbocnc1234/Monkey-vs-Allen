using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : IEffect, IOnDamageTaken
{
    int amount;
    public Shield(IEntity owner, int amount) : base(owner, 5) {
        this.amount = amount;
    }
    public void OnDamageTaken(DamageContext ctx) {
        if (ctx.defender != owner) {
            return;
        }
        if(ctx.amount >= amount) {
            ctx.amount -= amount;
            DestroyThis();
        }
        else {
            amount -= ctx.amount;
            ctx.amount = 0;
        }
    }
    public override bool IsIdentical(IEffect effect) {
        return false;
    }
}
