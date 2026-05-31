using UnityEngine;

// [Wrapper] Phase 4: Visual events delegated to wrapper
public class AlienWithHelmentAppearance : EntityAppearance {
    public Transform helmet;
    public SkillSO helmetSO;
    public override void Initialize(EntityModel model) {
        base.Initialize(model);
        Shield effect = ((Entity)model.e).GetBehaviour<AlienWithHelmentInitializer>().shield;
        effect.OnDeath += () => {
            var dropBodyPart = helmet.gameObject.AddComponent<DropBodyPart>();
            dropBodyPart.Initialize(model.e.lane, 3);
        };
        effect.OnDamageTaken += (amount) => StartCoroutine(FlashWhite.FlashRoutine(new SpriteRenderer[] { helmet.GetComponent<SpriteRenderer>() }, amount));
    }
}