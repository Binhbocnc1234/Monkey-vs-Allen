using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Effect, IOnDamageTaken
{
    public Shield(int amount) : base(5) {
        this.strength = amount;
    }
    public void OnDamageTaken(DamageContext ctx) {
        if (ctx.defender != owner) {
            return;
        }
        if(ctx.amount >= strength) {
            ctx.amount -= strength;
            DestroyThis();
        }
        else {
            strength -= (int)ctx.amount;
            ctx.amount = 0;
        }
    }
    public override bool IsIdentical(Effect effect) {
        return false;
    }
}
