using UnityEngine;

public class GridPresenter : MonoBehaviour
{
    public static GridPresenter Ins { get; private set; }

    [SerializeField] private CellView cellViewPrefab;

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

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                ICell cell = grid.GetCell(i, j);
                if (cell == null) continue;

                Vector3 worldPos = grid.GridToWorldPosition(i, j);
                CellView cellView = Instantiate(cellViewPrefab, worldPos, Quaternion.identity, transform);
                cellView.Bind(cell);
            }
        }

        // Initialize Sliding Camera using the grid boundaries
        if (grid.bounds != null)
        {
            SlidingCamera.Ins.Initialize(grid.bounds.right, grid.bounds.left);
        }
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
