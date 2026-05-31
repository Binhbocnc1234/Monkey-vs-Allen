using UnityEngine;

public class MonkeySolidarity : PassiveSkill, IOnApply {
    Timer bonusTimer;
    private EntitySO monkeySO;
    public MonkeySolidarity(SkillSO skillSO, EntitySO monkeySO) : base(skillSO) {
        this.monkeySO = monkeySO;
    }
    public override void Update(float deltaTime) {
        base.Update(deltaTime);
        if(bonusTimer != null && bonusTimer.Count(deltaTime)) {
            bonusTimer = null;
            IEntityRegistry.Ins.CreateEntity(new EntitySetting {
                so = monkeySO, x = owner.team == Team.Left ? -1 : IGrid.Ins.width, lane = owner.lane, team = owner.team
            });
        }
    }
    public override void OnApply() {
        base.OnApply();
        if(Utilities.RollAndGetResult(GetStat("Chance"))) {
            bonusTimer = new Timer(1, false);
        }
    }
}
