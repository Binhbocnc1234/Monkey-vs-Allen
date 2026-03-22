using UnityEngine;


public class BigWatermelon : Bullet {
    public bool isFalling;
    public SkillSO skillSO;
    protected override void OnHit(IEntity target) {
        base.OnHit(target);
        AreaDamage.CreateAreaDamage(lane, target.gridPos.x, target.GetSkillStat(skillSO, "Damage"), AreaDamage.Range.Medium, owner);
    }
}