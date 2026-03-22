using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(GridLayoutGroup))]
public class GridHeightCorrector : MonoBehaviour
{
    private GridLayoutGroup grid;
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
        if(grid == null) {
            grid = GetComponent<GridLayoutGroup>();
        }
        if(rect == null) rect = GetComponent<RectTransform>();
        if (grid.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
            return;

        int activeCount = 0;
        foreach (Transform child in transform)
            if (child.gameObject.activeSelf)
                activeCount++;

        if (activeCount == 0)
        {
            SetHeight(0);
            return;
        }

        int columns = grid.constraintCount;
        int rows = Mathf.CeilToInt((float)activeCount / columns);

        float height =
            grid.padding.top +
            grid.padding.bottom +
            rows * grid.cellSize.y +
            (rows - 1) * grid.spacing.y;

        SetHeight(height);
    }

    void SetHeight(float height)
    {
        Vector2 size = rect.sizeDelta;
        size.y = height;
        rect.sizeDelta = size;
    }
}