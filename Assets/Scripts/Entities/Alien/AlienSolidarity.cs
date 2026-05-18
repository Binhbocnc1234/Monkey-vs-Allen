using System.Collections.Generic;

public class AlienSolidarity : PassiveSkill, IModifyStat {
    public AlienSolidarity(SkillSO so) : base(so){}
    public List<StatModifier> ModifyStat() {
        return new() { new StatModifier(Operator.Addition, ST.Armor, GetBasicAlienCnt() * GetStat("ArmorIncreasedAmount")) };
    }
    int GetBasicAlienCnt() {
        int basicAlienCnt = 0;
        foreach(IEntity e in IEntityRegistry.Ins.GetEntities()) {
            if(e.GetSO().name == "Basic Alien" && e.team == owner.team) {
                basicAlienCnt++;
            }
        }
        return basicAlienCnt;
    }
}