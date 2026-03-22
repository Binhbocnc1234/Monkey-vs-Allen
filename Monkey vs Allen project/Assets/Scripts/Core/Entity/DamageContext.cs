public class DamageContext {
    public float amount;
    public float damageMultiplier = 1;
    public readonly IEntity attacker;
    public readonly IEntity defender;
    public readonly bool isMagicalDamage;
    public float penetrationAmount;
    public DamageContext(float amount, IEntity attacker, IEntity defender,
        int penetrationAmount, bool isMagicalDamage = false) : this(amount, attacker, defender, isMagicalDamage) {
        this.amount = amount;
        this.attacker = attacker;
        this.isMagicalDamage = isMagicalDamage;
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
    }
}
