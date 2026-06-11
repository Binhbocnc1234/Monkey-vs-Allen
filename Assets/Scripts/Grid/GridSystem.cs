using System.Collections.Generic;
using UnityEngine;

public class GridSystem : IGrid {
    public static new GridSystem Ins { get; private set; }

    public override void Clear(){
        cells = null;
    }

    /// <summary>
    /// Initializes the logical grid cells
    /// </summary>
    public override void Initialize(int width, bool[] openLanes)
    {
        Ins = this;
        IGrid.Ins = this;
        this.width = width;
        height = openLanes.Length;
        cells = new ICell[width, height];
        this.openLanes = (bool[])openLanes.Clone();
        
        // Bounds logic math
        bounds = new Bound(0, GridToWorldPosition(0, height - 1).y, 0, GridToWorldPosition(width - 1, 0).x);
        
        // Create pure logical cells
        for(int i = 0; i < width; ++i) {
            for(int j = 0; j < height; ++j) {
                cells[i, j] = new LogicalCell(i, j);
            }
        }
        Debug.Log("Successfully initialized logical GridSystem");
    }

    public override Vector2 GridToWorldPosition(Vector2 gridPos)
    {
        return GridToWorldPosition(gridPos.x, gridPos.y);
    }

    public override Vector2 GridToWorldPosition(float x, float y){
        return new Vector2(x * CELL_SIZE, y * CELL_SIZE);
    }

    /// <summary>
    /// Converts Unity world coordinates to grid coordinates.
    /// </summary>
    public override Vector2Int WorldToGridPosRounded(Vector2 worldPosition)
    {
        Vector2 localPosition = worldPosition;
        Vector2Int ans = new Vector2Int(
            Mathf.FloorToInt(localPosition.x / CELL_SIZE),
            Mathf.FloorToInt(localPosition.y / CELL_SIZE)
        );
        return ans;
    }

    public override Vector2 WorldToGridPos(Vector2 v) {
        return new Vector2(v.x / CELL_SIZE, v.y / CELL_SIZE);
    }

    public override ICell GetCell(int x, int y)
    {
        if (IsValidGridPosition(x, y))
        {
            return cells[x, y];
        }
        else{
            Debug.LogError($"GridSystem:GetCell({x}, {y}): Invalid position");
            return null;
        }
    }

    public override ICell GetCell(Vector2Int position)
    {
        return GetCell(position.x, position.y);
    }

    /// <summary>
    /// Checks if the given grid coordinates are within valid bounds.
    /// </summary>
    public override bool IsValidGridPosition(float x, float y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    
    /// <summary>
    /// Overloaded version that takes Vector2Int coordinates.
    /// </summary>
    public bool IsValidGridPosition(Vector2Int position)
    {
        return IsValidGridPosition(position.x, position.y);
    }

    public override List<int> GetOpenLanes(){
        List<int> openLanesList = new();
        if (openLanes != null) {
            for(int i = 0; i < openLanes.Length; ++i) {
                if(openLanes[i]) {
                    openLanesList.Add(i);
                }
            }
        }
        return openLanesList;
    }
}
