using System.Collections.Generic;

public interface IAssessable {
    public List<APModifier> GetAssessPoint();
}
public enum APType {
    Danger,
    Defend,
    Debuff,
    Buff,
    NeedProtection, // Economy, Target, they have no contribution to war but still need to protect
}
public class APModifier {
    public Operator op;
    public APType type;
    public float value;
    public APModifier(Operator op, APType type, float value) {
        this.op = op;
        this.value = value;
        this.type = type;
    }
}