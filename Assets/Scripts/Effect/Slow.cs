using System.Collections.Generic;

public class Slow : Effect, IModifyStat{
    public float slowStrength;
    public Slow(float duration, float strenth) : base(duration) {
        this.slowStrength = strength;
    }
    public List<StatModifier> ModifyStat() {
        return new() { new StatModifier(Operator.Multiply, ST.MoveSpeed, 0.5f) };
    }
}