using UnityEngine;

public class Target : Entity {
    public override float GetAssessPoint(APType type) {
        return base.GetAssessPoint(APType.Danger) + (type == APType.NeedProtection ? 150 : 0);
    }
}