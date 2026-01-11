
using UnityEngine;

public static class RectTransformExtensions {
    public static void SetLeft(this RectTransform rt, float left) {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right) {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top) {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom) {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}
public static class TransformExtensions
{
    public static void ClearChildren(this Transform go)
    {
        if (go == null) return;
        for (int i = go.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(go.transform.GetChild(i).gameObject);
        }
    }
}