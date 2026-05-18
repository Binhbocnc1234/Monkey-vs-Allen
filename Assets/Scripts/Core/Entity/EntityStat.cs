using UnityEngine;

[System.Serializable]
public class EntityStat {
    [SerializeField] private ST statType;
    public float amount;
    public float multiplier = 1;
    public EntityStat(ST statType, float amount) {
        this.statType = statType;
        this.amount = amount;
    }
    public ST GetStatType() => statType;
}

public enum ST {
    Health,
    MaxHealth,
    Strength,
    MagicPower,
    Range,
    AttackSpeed,
    MoveSpeed, // How many cells can the Entity pass through in 1 second?
    LifeSteal, // Unit: percent
    Armor,
    MagicResistance,
    Penetration,
    MagicPenetration,
    MoveSM, // MoveSpeedMultiplier
    CriticalChance,
    VirtualHealth,
}