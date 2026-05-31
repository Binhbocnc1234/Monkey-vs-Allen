using System;
using System.Collections.Generic;
using UnityEngine;

public class AlienWithHelmentInitializer : IBehaviour, IInitialize {
    private IEntity entity;
    public SkillSO helmetSO;
    [NonSerialized] public Shield shield;
    private List<APModifier> points;
    public void Initialize(IEntity e) {
        entity = e;
    }
    public void Initialize() {
        shield = new Shield(-1, (int)entity.GetSkillStat(helmetSO, "Shield"));
        entity.GetEffectable().ApplyEffect(shield);
        points = new();
        points.AddRange(shield.GetAssessPoint());
        shield.OnDeath += () => {
            if(entity.level >= 3) {
                // [Wrapper] Create shock wave
                // Also need logic
            }
        };
    }
    public override List<APModifier> GetAssessPoint() {
        return new(){new APModifier(Operator.Addition, APType.Defend, entity.GetSkillStat(helmetSO, "Shield"))};
    }

    public override bool CanActive() {
        return false;
    }

    public override int GetPriority() {
        return -1;
    }

    public override string GetAnimatorStateName() {
        throw new NotImplementedException();
    }

}
