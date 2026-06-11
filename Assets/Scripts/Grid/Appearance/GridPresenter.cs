using UnityEngine;

public class GridPresenter : MonoBehaviour
{
    public static GridPresenter Ins { get; private set; }

    [SerializeField] private CellView cellViewPrefab;

    private CellView[,] cellViews;

    private void Awake()
    {
        Ins = this;
    }

    public void InitializeGrid(IGrid grid)
    {
        Clear();

        if (grid == null) return;

        int width = grid.width;
        int height = grid.height;
        cellViews = new CellView[width, height];

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                ICell cell = grid.GetCell(i, j);
                if (cell == null) continue;

                Vector3 worldPos = grid.GridToWorldPosition(i, j);
                CellView cellView = Instantiate(cellViewPrefab, worldPos, Quaternion.identity, transform);
                cellView.Bind(cell);
                cellViews[i, j] = cellView;
            }
        }

        // Initialize Sliding Camera using the grid boundaries
        if (grid.bounds != null)
        {
            SlidingCamera.Ins.Initialize(grid.bounds.right, grid.bounds.left);
        }
    }

    public CellView GetCellView(int x, int y)
    {
        if (cellViews != null && x >= 0 && x < cellViews.GetLength(0) && y >= 0 && y < cellViews.GetLength(1))
        {
            return cellViews[x, y];
        }
        return null;
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
