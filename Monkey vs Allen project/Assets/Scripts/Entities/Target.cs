using UnityEngine;

public class Target : Entity {
    public override float GetAssessPoint(APType type) {
        return base.GetAssessPoint(type) + (type == APType.NeedProtection ? 150 : 0);
    }
}