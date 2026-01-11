using UnityEngine;
public static class EffectFactory {
    public static IEffect Get(TraitType type, IEntity e) {
        return type switch {
            TraitType.Deadly => new DeadlyMark(e),
            TraitType.Poisoning => new Poisoning(e),
            TraitType.OnFire => new OnFire(e),
            TraitType.Armored => new Armored(e, 1),
            TraitType.AntiTower => new AntiTower(e, -1),
            _ => null,
        };
    }
}