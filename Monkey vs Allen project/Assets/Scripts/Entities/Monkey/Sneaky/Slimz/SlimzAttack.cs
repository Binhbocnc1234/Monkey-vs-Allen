using UnityEngine;

public class SlimzAttack : RangedAttack {
    [SerializeField] private Poke pokePrefab;
    [SerializeField] private SkillSO skillSO;
    public bool isNextTimeIsPoke = false;
    protected override void MakeDamageInstantly() {
        if(isNextTimeIsPoke) {
            Poke newBullet = Instantiate(pokePrefab, firePoint.transform.position, Quaternion.identity);
            newBullet.Initialize(e, e[ST.Strength],
            e.GetSkillStat(skillSO, "StunDuration"), isDebuffApplied: e.level >= 3);
        }
        else {
            base.MakeDamageInstantly();
        }
    }
    public void NextAttackBecomePoke() {
        isNextTimeIsPoke = true;
    }
}