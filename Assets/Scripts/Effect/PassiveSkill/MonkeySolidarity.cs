using UnityEngine;

public class MonkeySolidarity : PassiveSkill, IOnApply {
    Timer bonusTimer;
    public MonkeySolidarity(SkillSO skillSO) : base(skillSO) {

    }
    public override void Update() {
        base.Update();
        if(bonusTimer != null && bonusTimer.Count()) {
            bonusTimer = null;
            IEntityRegistry.Ins.CreateEntity(EntitySO.GetSOByName("Basic Monkey"),
                owner.lane, owner.team);
        }
    }
    public override void OnApply() {
        base.OnApply();
        if(Utilities.RollAndGetResult(GetStat("Chance"))) {
            bonusTimer = new Timer(1, false);
        }
    }
}
