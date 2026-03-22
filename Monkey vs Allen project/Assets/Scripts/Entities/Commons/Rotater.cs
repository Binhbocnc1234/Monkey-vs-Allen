using System.Collections.Generic;
using UnityEngine;

public class VectorRef {
    public float x, y;
    public VectorRef(float x, float y) {
        this.x = x;
        this.y = y;
    }
}
/*
Liệt kê những lý do cần thay đổi rotate
Khi bị Hyponetized : flip.x 
Khi dính Effect giantic làm cơ thể to lên, tăng cả scale.x và scale.y
Khi là Enemy thì scale.x sẽ cần phải đảo
Khi là constructor ngược chiều thì cần phải đảo
*/
public class Rotater : MonoBehaviour {
    public bool flipX, flipY;
    [ReadOnly] public Vector2 originalScale;
    public List<VectorRef> localScaleModifier;
    // private IEntity e;
    void Awake() {
        originalScale = transform.localScale;
    }
    void Update() {
        localScaleModifier = new();
        Vector2 finalScale = originalScale;
        foreach(VectorRef v in localScaleModifier) {
            finalScale.x *= v.x;
            finalScale.y *= v.y;
        }
        if(flipX) {
            finalScale.x *= -1;
        }
        if(flipY) {
            finalScale.y *= -1;
        }
        transform.localScale = finalScale;
    }
    public void FlipX() {
        flipX = !flipX;
    }
    public void FlipY() {
        flipY = !flipY;
    }
}