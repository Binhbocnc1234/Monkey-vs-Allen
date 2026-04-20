using System.Collections.Generic;

public class DoubleDamageEffect : Effect, IDamageOutputModifier {
    public void ModifyDamage(DamageContext ctx) {
        ctx.AddModifier(new DamageModifier(Operator.Multiply, 2));
    }
    public override List<APModifier> GetAssessPoint() {
        return new() { new APModifier(Operator.Addition, APType.Buff, 30) };
    }
}