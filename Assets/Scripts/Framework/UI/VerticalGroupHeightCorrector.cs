using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(VerticalLayoutGroup))]
public class VerticalGroupHeightCorrector : MonoBehaviour
{
    private VerticalLayoutGroup layout;
    private RectTransform rect;

    void OnEnable()
    {
        UpdateHeight();
    }

    void OnTransformChildrenChanged()
    {
        UpdateHeight();
    }

    public void UpdateHeight()
    {
        if (layout == null || rect == null) {
            layout = GetComponent<VerticalLayoutGroup>();
            rect = GetComponent<RectTransform>();
        }
        float totalHeight = 0f;
        int activeCount = 0;

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf)
                continue;

            RectTransform childRect = child as RectTransform;
            if (childRect == null)
                continue;

            totalHeight += childRect.sizeDelta.y;
            activeCount++;
        }

        if (activeCount > 1)
            totalHeight += layout.spacing * (activeCount - 1);

        totalHeight += layout.padding.top + layout.padding.bottom;

        SetHeight(totalHeight);
    }

    void SetHeight(float height)
    {
        Vector2 size = rect.sizeDelta;
        size.y = height;
        rect.sizeDelta = size;
    }
}