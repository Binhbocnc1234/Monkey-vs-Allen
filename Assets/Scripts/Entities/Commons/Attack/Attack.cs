[System.Serializable]
public class Attack : AttackBase {
    protected override void WhenAttackReady() {
        ApplyDirectDamage();
    }
}
