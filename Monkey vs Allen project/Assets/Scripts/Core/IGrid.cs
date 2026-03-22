using UnityEngine;

public abstract class IGrid : Singleton<IGrid>{
    [Header("Grid Settings")]
    public const float CELL_SIZE = 2;  // Size of each grid cell in world units
    public int height{ get; protected set; } // Number of vertical rows
    public int width{ get; protected set; } // Number of horizontal lanes
    public ICell[,] cells;
    public Bound bounds;
    public bool[] openLanes;
    public abstract bool IsValidGridPosition(float x, float y);
    public abstract Vector2 GridToWorldPosition(Vector2 gridPos);
    public abstract Vector2 GridToWorldPosition(float x, float y);
    public abstract Vector2 WorldToGridPos(Vector2 v);
    public abstract Vector2Int WorldToGridPosRounded(Vector2 worldPosition);
    public abstract ICell GetCell(Vector2Int gridPos);
    public abstract ICell GetCell(int x, int y);
    public abstract void CreateCell(ICell cellPrefab, int x, int y);
    // public abstract void PlaceBlockAtLane(int y, int count = 1);
    public abstract void Initialize(LevelSO so);
    public abstract void Clear();
}
public abstract class ICell : MonoBehaviour {
    public Vector2Int gridPosition { get; protected set; }
    public bool hasBlock { get; protected set; } = false;
    public bool occupiedByTower = false;
    public SpriteRenderer blockRenderer;
    public abstract void PlaceBlock(Sprite blockSprite);
}
// Nếu tương lai bản thân block có hiệu ứng đặc biệt, ví dụ tower đừng trên đó 