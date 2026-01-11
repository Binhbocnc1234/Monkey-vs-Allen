public class DamageContext {
    public int amount;
    public readonly IEntity attacker;
    public readonly IEntity defender;
    public readonly bool isMagicalDamage;
    public int penetrationAmount;
    public DamageContext(int amount, IEntity attacker, IEntity defender, bool isMagicalDamage = false, int penetrationAmount = 0) {
        this.amount = amount;
        this.attacker = attacker;
        this.isMagicalDamage = isMagicalDamage;
        this.penetrationAmount = penetrationAmount;
    }
}
