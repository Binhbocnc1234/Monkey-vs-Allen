
using System;
using System.Collections.Generic;

public class ArmorBreak : Effect, IStackable, IModifyStat, IOnApply {
    public ArmorBreak(IEntity owner) : base(5) {

    }
    public void OnApply() {
        strength += 1;
    }
    public List<StatModifier> ModifyStat() {
        return new() {new StatModifier(Operator.Addition, ST.Armor, -strength*4) };
    }
    public void Stack(int amount) {
        if (strength < 4) {
            OnApply();
        }
    }
}