using UnityEngine;
public static class EffectFactory {
    public static Effect Get(EffectType type, IEntity e) {
        return type switch {
            EffectType.Deadly => new DeadlyMark(),
            EffectType.IronBody => new IronBody(-1, 1),
            EffectType.AntiTower => new AntiTower(-1),
            _ => null,
        };
    }
}