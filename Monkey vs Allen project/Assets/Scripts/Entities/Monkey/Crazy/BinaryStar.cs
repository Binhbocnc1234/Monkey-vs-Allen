using UnityEngine;

public class DoubleDamageEffect : GlobalEffect, IDamageOutputModifier {
    public void ModifyDamage(DamageContext ctx) {
        ctx.amount *= 2;
    }
}
public class BinaryStar : Entity {
    private DoubleDamageEffect globalEffect;
    void Start() {
        SingletonRegister.Get<GlobalEffectManager>().AddEffect(globalEffect);
    }
    public override void Die() {
        base.Die();
        SingletonRegister.Get<GlobalEffectManager>().RemoveEffect(globalEffect);
    }
}