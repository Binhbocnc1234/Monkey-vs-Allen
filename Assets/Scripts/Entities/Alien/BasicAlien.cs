using UnityEngine;

public class BasicAlien : MonoBehaviour, IInitialize {
    public SkillSO skillSO;
    public void Initialize() {
        IEntity e = GetComponent<IEntity>();
        if(e.level >= 3) {
            e.GetEffectable().ApplyEffect(new AlienSolidarity(skillSO));
        }
    }
}