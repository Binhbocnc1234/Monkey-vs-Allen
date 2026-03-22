using UnityEngine;

public class BasicMonkey : MonoBehaviour, IInitialize {
    SkillSO solidaritySkill;
    public void Initialize() {
        IEntity e = GetComponent<IEntity>();
        if(e.level >= 3) {
            e.GetEffectable().ApplyEffect(new MonkeySolidarity(solidaritySkill));
        }
    }
}