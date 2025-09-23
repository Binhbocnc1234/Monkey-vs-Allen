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
public enum Direction{
    Up,
    Down,
    Left,
    Right
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
    public static int Convert(CoolDownType coolDownType){
        switch(coolDownType){
            case CoolDownType.None : return 0;
            case CoolDownType.Low : return 5;
            case CoolDownType.Medium : return 20;
            case CoolDownType.High : return 50;
            default : return 0;
        }
    }
}
public enum Place{
    Garden,
    Sky,
    Villa,
    HauntedVilla,
    None
}
public enum EntityState{
    Idle,
    Walk,
    Attacking,
    Death,
    Frozen
}
public enum AttackType
{
    Melee,
    Ranged,
    Area
}

public enum WaveState
{
    Pause,
    Active
}
public enum EnemyTribe
{
    Basic,
    Pet,
    Mechanic,
    Imp, 
    Gargatuar, 
    Cosmic // Immune to control effect
}
public enum EntityEffectType{
    Immune,
    Untrickable,
    None
}