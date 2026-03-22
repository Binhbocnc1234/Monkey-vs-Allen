using System.Collections.Generic;
using System.Linq;

public class GeneralStatEffect : Effect, IModifyStat {
    List<StatModifier> modifiers;
    public GeneralStatEffect(List<StatModifier> modifiers, int duration = -1) : base(duration) {
        this.modifiers = modifiers;
    }
    public List<StatModifier> ModifyStat() {
        return modifiers;
    }
}