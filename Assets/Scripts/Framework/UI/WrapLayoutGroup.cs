using UnityEngine;
using UnityEngine.UI;

public class WrapLayoutGroup : LayoutGroup
{
    public float spacing = 10f;
    public float childHeight = 50f;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        DoLayout();
    }

    public override void CalculateLayoutInputVertical()
    {
        DoLayout();
    }

    public override void SetLayoutHorizontal() { }
    public override void SetLayoutVertical() { }

    void DoLayout()
    {
        float width = rectTransform.rect.width;
        float x = padding.left, y = padding.top;
        float lineHeight = childHeight + spacing;

        foreach (RectTransform child in rectTransform)
        {

            float w = child.rect.width + spacing;

            if (x + w > width)
            {
                x = padding.left;
                y += lineHeight;
            }

            SetChildAlongAxis(child, 0, x, w);
            SetChildAlongAxis(child, 1, y, childHeight);

            // Force uniform height
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, childHeight);

            x += w;
        }

        SetLayoutInputForAxis(y + lineHeight + padding.bottom, y + lineHeight, -1, 1);
    }
}
