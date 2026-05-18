using System.Collections.Generic;

public class DamageContext {
    public float amount{ get; private set; }
    public float damageMultiplier = 1;
    public readonly bool isCritical = false;
    public readonly IEntity attacker;
    public readonly IEntity defender;
    public readonly bool isMagicalDamage;
    public float penetrationAmount;
    private List<DamageModifier> modifiers = new();
    public DamageContext(float amount, IEntity attacker, IEntity defender,
        int penetrationAmount, bool isMagicalDamage = false) : this(amount, attacker, defender, isMagicalDamage) {
        if(isMagicalDamage) {
            this.penetrationAmount = penetrationAmount;
        }
        else {
            this.penetrationAmount = penetrationAmount;
        }
    }

    public DamageContext(float amount, IEntity attacker, IEntity defender, bool isMagicalDamage = false) {
        this.amount = amount;
        this.attacker = attacker;
        this.isMagicalDamage = isMagicalDamage;
        if(isMagicalDamage) {
            this.penetrationAmount = attacker[ST.MagicPenetration];
        }
        else {
            this.penetrationAmount = attacker[ST.Penetration];
        }
        if(Utilities.RollAndGetResult(attacker[ST.CriticalChance])) {
            isCritical = true;
            damageMultiplier *= 2;
        }
    }
    public void AddModifier(DamageModifier modifier) {
        modifiers.Add(modifier);
    }
    public void Flush() {
        modifiers.Sort((a, b) => a.op.CompareTo(b.op));
        foreach(DamageModifier modifier in modifiers) {
            if(modifier.op == Operator.Addition) {
                amount += modifier.amount;
            }
            else if (modifier.op == Operator.Multiply) {
                amount *= modifier.amount;
            }
        }
    }
}

public enum DamageType {
    Magical,
    Physical,
    TrueDamage,
}

public class DamageModifier {
    public float amount;
    public Operator op;
    public DamageModifier(Operator op, float amount) {
        this.op = op;
        this.amount = amount;
    }
}