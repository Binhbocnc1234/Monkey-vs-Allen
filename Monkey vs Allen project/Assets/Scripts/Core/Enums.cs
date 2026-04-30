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
            case CoolDownType.None:
                Debug.LogError("[EnumConverter] cooldownType == None");
                return 0;
            case CoolDownType.Low: return 7;
            case CoolDownType.Medium: return 20;
            case CoolDownType.High: return 60;
            default: return 0;
        }
    }
    public static Color FromRarityToColor(CardRarity rarity) {
        switch(rarity) {
            case CardRarity.Common: return Color.white;
            case CardRarity.Occasional: return Color.green;
            case CardRarity.Rare: return Color.blue;
            case CardRarity.Epic: return Color.magenta;
            case CardRarity.Legendary: return Color.red;
            case CardRarity.Exotic: return Color.yellow;
            default: return Color.white;
        }
    }
    public static Team GetOppositeSide(Team team) {
        if(team == Team.Left) return Team.Right;
        else { return Team.Left; }
    }
    public static float GetBasePosition(Team team) {
        return team == Team.Right ? IGrid.Ins.width : -1;
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
/// <summary>
/// Là một thuộc tính của Entity, team quyết định xem Entity sẽ spawn ở bên trái hay bên phải camera 
/// và di chuyển theo hướng nào. 
/// Ở chế độ chơi với máy, người chơi sẽ luôn điều khiển phe Player, còn Enemy sẽ được điều khiển bởi AI
/// Nhưng ở chế độ Multiplayer, mỗi người chơi sẽ điều khiển 1 phe
/// </summary>
public enum Team {
    Left,
    Right,
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
public enum Faction {
    Alien, Monkey, Tower
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

public enum Language {
    English,
    Vietnamese,
    Spanish,
    Portuguese,
    Indonesian,
}

public enum Operator {
    Multiply,
    Addition,
    Override,
}

public enum BuildTime {
    Low,
    High
}