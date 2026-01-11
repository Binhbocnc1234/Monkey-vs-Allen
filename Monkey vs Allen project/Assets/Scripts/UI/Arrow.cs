using UnityEngine;

public class Arrow : MonoBehaviour
{
    Transform tf;
    public Direction pointingDirection;
    public float magnitude = 1f;   // world units
    public float speed = 2f;       // oscillation speed

    Vector3 basePos;

    void Start() {
        tf = transform;
        basePos = tf.position;

        switch (pointingDirection) {
            case Direction.Right: break;
            case Direction.Up:    tf.Rotate(0, 0, 90);  break;
            case Direction.Left:  tf.Rotate(0, 0, 180); break;
            case Direction.Down:  tf.Rotate(0, 0, 270); break;
            default: Debug.LogError("Bad pointingDirection"); break;
        }
    }

    void Update() {
        float offset = -Mathf.Cos(Time.time * speed) * magnitude;
        Vector3 dir = DirectionToVector(pointingDirection);
        tf.position = basePos + dir * offset;
    }

    Vector3 DirectionToVector(Direction d) {
        switch (d) {
            case Direction.Right: return Vector3.right;
            case Direction.Up:    return Vector3.up;
            case Direction.Left:  return Vector3.left;
            case Direction.Down:  return Vector3.down;
            default: return Vector3.zero;
        }
    }
}
