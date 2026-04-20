using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Effect, IDamageInputModifier, IOnApply
{
    public event Action<int> OnDamageTaken;
    public Shield(float duration, int shieldAmount) : base(duration, shieldAmount) {
    }
    public void OnApply() {
        
    }
    public void ModifyDamage(DamageContext ctx) {
        OnDamageTaken?.Invoke((int)ctx.amount);
        if(ctx.amount >= strength) {
            ctx.AddModifier(new DamageModifier(Operator.Addition, -strength));
            DestroyThis();
        }
        else {
            strength -= (int)ctx.amount;
            ctx.AddModifier(new DamageModifier(Operator.Addition, -ctx.amount));
        }
    }
    public override bool IsIdentical(Effect effect) {
        return false;
    }
    public override List<APModifier> GetAssessPoint() {
        return new() { new APModifier(Operator.Addition, APType.Defend, strength/7) };
    }
}
