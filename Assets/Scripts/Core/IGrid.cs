using System.Collections.Generic;
using UnityEngine;

public abstract class IGrid {
    public static IGrid Ins { get; set; }

    [Header("Grid Settings")]
    public const float CELL_SIZE = 2;  // Size of each grid cell in world units
    public int height { get; protected set; } // Number of vertical rows
    public int width { get; protected set; } // Number of horizontal lanes
    public ICell[,] cells;
    public Bound bounds;
    public bool[] openLanes;

    public abstract void Initialize(int width, bool[] openLanes);
    public abstract bool IsValidGridPosition(float x, float y);
    public abstract Vector2 GridToWorldPosition(Vector2 gridPos);
    public abstract Vector2 GridToWorldPosition(float x, float y);
    public abstract Vector2 WorldToGridPos(Vector2 v);
    public abstract Vector2Int WorldToGridPosRounded(Vector2 worldPosition);
    public abstract ICell GetCell(Vector2Int gridPos);
    public abstract ICell GetCell(int x, int y);
    public abstract void Clear();
    public abstract List<int> GetOpenLanes();
}

public abstract class ICell {
    public Vector2Int gridPosition { get; protected set; }
    public bool hasBlock { get; protected set; } = false;
    public bool occupiedByTower = false;

    public event System.Action<ICell> OnStateChanged;
    public event System.Action<ICell, Sprite> OnSpriteChanged;

    public abstract void PlaceBlock(Sprite blockSprite);
    public abstract void SetSpriteOnly(Sprite sprite);

    protected void NotifyStateChanged() => OnStateChanged?.Invoke(this);
    protected void NotifySpriteChanged(Sprite sprite) => OnSpriteChanged?.Invoke(this, sprite);
}