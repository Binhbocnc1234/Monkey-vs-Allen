public class BasicAlien : IInitialize {
    private readonly IEntity entity;
    private readonly SkillSO skillSO;
    public BasicAlien(IEntity entity, SkillSO skillSO) {
        this.entity = entity;
        this.skillSO = skillSO;
    }
    public void Initialize() {
        if(entity.level >= 3) {
            entity.GetEffectable().ApplyEffect(new AlienSolidarity(skillSO));
        }
    }
}
