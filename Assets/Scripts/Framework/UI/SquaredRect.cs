using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class SquareRect : MonoBehaviour
{
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        UpdateSize();
    }

    void Update()
    {
        UpdateSize();
    }

    void UpdateSize()
    {
        if(rect == null) return;
        // Debug.Log("run");
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.height);
    }
}
