using System;
using UnityEngine;


public static class UnityExtension {
    public static void DestroyAllChildren(this Transform t) {
        for(int i = t.childCount - 1; i >= 0; i--)
            GameObject.Destroy(t.GetChild(i).gameObject);
    }
    public static void AssignXPos(this Transform t, float x) {
        Vector3 oldPos = t.position;
        oldPos.x = x;
        t.position = oldPos;
    }
    public static void AssignYPos(this Transform t, float y) {
        Vector3 oldPos = t.position;
        oldPos.y = y;
        t.position = oldPos;
    }
    public static void FlipLocalScaleX(this Transform t) {
        Vector3 oldLocalScale = t.localScale;
        oldLocalScale.x *= -1;
        t.localScale = oldLocalScale;
    }
    public static void FlipLocalScaleY(this Transform t) {
        Vector3 oldLocalScale = t.localScale;
        oldLocalScale.y *= -1;
        t.localScale = oldLocalScale;
    }
    public static void AdjustRendererAlpha(this SpriteRenderer renderer, float diff) {
        Color c = renderer.color;
        c.a += diff;
        if(c.a <= 0) {
            c.a = 0;
        }
        else if(c.a > 1) {
            c.a = 1;
        }
        renderer.color = c;
    }
    public static void AssignRendererAlpha(this SpriteRenderer renderer, float value) {
        Color c = renderer.color;
        c.a = value;
        if(c.a <= 0) {
            c.a = 0;
        }
        else if(c.a > 1) {
            c.a = 1;
        }
        renderer.color = c;
    }
    public static Vector3 GetRelativePositionTo(this Transform a, Transform other) {
        return other.transform.position - a.transform.position;
    }
}
