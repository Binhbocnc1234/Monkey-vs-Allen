using System.Collections.Generic;

public class AlienSolidarity : PassiveSkill, IModifyStat {
    public AlienSolidarity(SkillSO so) : base(so){}
    public List<StatModifier> ModifyStat() {
        int basicAlienCnt = 0;
        foreach(IEntity e in IEntityRegistry.Ins.GetEntities()) {
            if(e.GetSO().name == "Basic Alien" && e.team == owner.team) {
                basicAlienCnt++;
            }
        }
        return new() { new StatModifier(Operator.Addition, ST.Armor, basicAlienCnt*GetStat("ArmorIncreasedAmount")) };
    }
}