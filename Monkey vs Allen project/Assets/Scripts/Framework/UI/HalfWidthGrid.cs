using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[ExecuteAlways]
public class HalfWidthGrid : MonoBehaviour
{
    public int columns = 2; // Number of columns

    private GridLayoutGroup grid;
    private RectTransform rect;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();
        UpdateCellSize();
    }

    void Update()
    {
        // Update in editor and runtime if size changes
        UpdateCellSize();
    }

    void UpdateCellSize()
    {
        if (grid == null || rect == null) return;

        float parentWidth = rect.rect.width;
        float spacingTotal = grid.spacing.x * (columns - 1);
        float cellWidth = (parentWidth - spacingTotal - grid.padding.left - grid.padding.right) / columns;

        Vector2 size = grid.cellSize;
        size.x = cellWidth;
        grid.cellSize = size;
    }
}
