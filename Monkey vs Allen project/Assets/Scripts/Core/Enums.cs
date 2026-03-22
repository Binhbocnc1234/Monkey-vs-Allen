using UnityEngine;

public enum GameState
{
    ChoosingCard,
    Prepare,
    Fighting,
    Paused,
    GameOver,
    Victory
}

public enum CardRarity {
    Common,
    Occasional,
    Rare,
    Epic,
    Exotic,
    Legendary
}
public enum CoolDownType{
    None,
    Low,
    Medium,
    High, 
    Heavy,
}
public static class EnumConverter{
    public static Vector2 Convert(Direction direction){
        switch(direction){
            case Direction.Up : return Vector2.up;
            case Direction.Down : return Vector2.down;
            case Direction.Right : return Vector2.right;
            case Direction.Left : return Vector2.left;
            default: return Vector2.zero;
        }
    }
    public static int Convert(CoolDownType coolDownType) {
        switch(coolDownType) {
            case CoolDownType.None: return 0;
            case CoolDownType.Low: return 7;
            case CoolDownType.Medium: return 20;
            case CoolDownType.High: return 60;
            default: return 0;
        }
    }
    public static Color FromRarityToColor(CardRarity rarity) {
        switch(rarity) {
            case CardRarity.Common : return Color.white;
            case CardRarity.Occasional : return Color.green;
            case CardRarity.Rare : return Color.blue;
            case CardRarity.Epic : return Color.magenta;
            case CardRarity.Legendary : return Color.red;
            case CardRarity.Exotic : return Color.yellow;
            default : return Color.white;
        }
    }
}
public enum Place {
    LovelyHouse,
    PrimalBreach,
    RampurVillage,
    BodiamCastle,
    Villa,
    FormusaFactory,
    CrystyCave,
    SkyCity,
}

public enum AttackType
{
    Melee,
    Ranged,
    Area
}
public enum GameClass {
    Hearty,
    Crazy,
    Brainy,
    Swarmy,
    Sneaky,
}
public enum Tribe {
    Basic,
    Target,
    Pet,
    Mechanic,
    Imp,
    Gargantuar,
    Cosmic, // Immune to control effect   
    Professional,
    Mutant,
    Ancient,
    Monkey,
    Allen,
    Tower,
    Medieval,
    Arrowy,
    Speary,
    Plant,
    Feral,
    JadeEmpire,
    Fruit,
    
}
public enum ST {
    Health,
    MaxHealth,
    Strength,
    MagicPower,
    Range,
    AttackSpeed,
    MoveSpeed, // How many cells can the Entity pass through in 1 second?
    LifeSteal,
    Armor,
    MagicResistance,
    Penetration,
    MagicPenetration,
    MoveSM, // MoveSpeedMultiplier
}
public enum Language {
    English,
    Vietnamese,
    Spanish,
    Portuguese,
    Indonesian,
}

public enum Operator {
    Addition,
    Multiply,
    Override,
}