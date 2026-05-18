using UnityEngine;

public class AlienWithHelmentAppearance : EntityAppearance, IInitialize {
    public Transform helmet;
    public SkillSO helmetSO;
    public override void Initialize() {
        base.Initialize();
        Shield effect = model.e.GetComponent<AlienWithHelmentInitializer>().shield;
        effect.OnDeath += () => {
            var dropBodyPart = helmet.gameObject.AddComponent<DropBodyPart>();
            dropBodyPart.Initialize(model.e.lane, 3);
        };
        effect.OnDamageTaken += (amount) => StartCoroutine(FlashWhite.FlashRoutine(new SpriteRenderer[] { helmet.GetComponent<SpriteRenderer>() }, amount));
    }
}