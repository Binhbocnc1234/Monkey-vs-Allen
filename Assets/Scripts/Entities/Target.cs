
using System.Collections.Generic;

public class Target : IBehaviour {

    public override bool CanActive() {
        return false;
    }
    public override List<APModifier> GetAssessPoint() {
        return new(){new APModifier(Operator.Addition, APType.NeedProtection, 150)};
    }
    public override string GetAnimatorStateName() {
        throw new System.NotImplementedException();
    }

    public override int GetPriority() {
        throw new System.NotImplementedException();
    }

}