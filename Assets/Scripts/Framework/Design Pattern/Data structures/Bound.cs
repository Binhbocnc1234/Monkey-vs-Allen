using UnityEngine;

[System.Serializable]
public class Bound {
    public float top, bottom, right, left;
    public Bound(float bottom, float top, float left, float right) {
        this.top = top;
        this.bottom = bottom;
        this.right = right;
        this.left = left;
    }
    public Vector2 RandomPosInBound() {
        return new Vector2(Random.Range(left, right), Random.Range(bottom, top));
    }
    public bool IsOutsideBound(Vector2 pos) {
        return pos.x < left || pos.x > right || pos.y < bottom || pos.y > top;
    }
}