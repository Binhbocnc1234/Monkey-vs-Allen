[System.Serializable]
public class MeleeAttack : AttackBase {
    protected override void WhenAttackReady() {
        ApplyDirectDamage();
    }
}
