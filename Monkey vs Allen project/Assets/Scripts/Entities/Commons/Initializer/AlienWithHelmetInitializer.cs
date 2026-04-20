using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class AlienWithHelmentInitializer : EntityBehaviour, IInitialize {
    public Transform helmet;
    public SkillSO helmetSO;
    private List<APModifier> points;
    public void Initialize() {
        Shield shield = new Shield(-1, (int)e.GetSkillStat(helmetSO, "Shield"));
        e.GetEffectable().ApplyEffect(shield);
        points = new();
        points.AddRange(shield.GetAssessPoint());
        shield.OnDeath += () => {
            var dropBodyPart = helmet.gameObject.AddComponent<DropBodyPart>();
            dropBodyPart.Initialize(e.lane, 3);
            if(e.level >= 3) {
                // Create shock wave
                CreateShockWave();
            }
        };
        shield.OnDamageTaken += (amount) => StartCoroutine(FlashWhite.FlashRoutine(new SpriteRenderer[] { helmet.GetComponent<SpriteRenderer>() }, amount));
    }
    void CreateShockWave() {

    }
    public List<APModifier> GetAssessPoint() {
        return new(){new APModifier(Operator.Addition, APType.Defend, e.GetSkillStat(helmetSO, "Shield"))};
    }
}