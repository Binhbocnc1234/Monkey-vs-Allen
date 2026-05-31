[System.Serializable]
public class BasicMonkey : IBehaviour, IInitialize {
    public SkillSO solidaritySkill;
    public EntitySO monkeySO;

    public override bool CanActive() {
        return false;
    }

    public override string GetAnimatorStateName() {
        throw new System.NotImplementedException();
    }

    public override int GetPriority() {
        return -1;
    }


    public void Initialize() {
        if(e.level >= 3) {
            e.GetEffectable().ApplyEffect(new MonkeySolidarity(solidaritySkill, monkeySO));
        }
    }
}
