using UnityEngine;

public abstract class IGrid : Singleton<IGrid>{
    [Header("Grid Settings")]
    public int height = 5; // Number of vertical rows
    public int width = 18; // Number of horizontal lanes
    public float cellSize = 2;  // Size of each grid cell in world units
    public GameObject blockPrefab;
    public abstract Vector2 GridToWorldPosition(Vector2Int gridPos);
    public abstract Vector2 GridToWorldPosition(int x, int y);
    public abstract bool IsValidGridPosition(int x, int y);
    public abstract Vector2Int WorldToGridPosition(Vector2 worldPosition);
    public abstract ICell GetCell(Vector2Int gridPos);
    public abstract ICell GetCell(int x, int y);
    public abstract void PlaceBlockAtLane(int y, int count = 1);
    public abstract void Initialize();
    public abstract void Clear();
}
public abstract class ICell: ReadOnlyMonoBehaviour{
    [Header("Cell Properties")]
    public Vector2Int gridPosition;
    public bool hasBlock;
    public bool hasEntity;
    
    [Header("References")]
    public GameObject blockObject;
    public GameObject entityObject;
    public abstract void PlaceBlock();
    public abstract GameObject PlaceObject(GameObject prefab);
    
}