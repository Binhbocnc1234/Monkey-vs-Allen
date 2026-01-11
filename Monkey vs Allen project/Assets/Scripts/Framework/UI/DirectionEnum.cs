using UnityEngine;

public enum Direction {
    Up,
    Down,
    Left,
    Right
}
public static class DirectionConverter {
    public static Vector2 Convert(Direction direction){
        switch(direction){
            case Direction.Up : return Vector2.up;
            case Direction.Down : return Vector2.down;
            case Direction.Right : return Vector2.right;
            case Direction.Left : return Vector2.left;
            default: return Vector2.zero;
        }
    }
}