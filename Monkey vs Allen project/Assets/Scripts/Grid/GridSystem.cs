using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// Manages the 2D grid system for the tower defense game.
/// Handles grid initialization, coordinate conversion, block placement, and entity positioning.
/// The grid serves as a position for all entities in the game (blocks, towers, monkeys, enemies).
/// 
/// Grid Layout (Horizontal Screen):
/// - Origin (0,0) is at the BOTTOM-LEFT corner
/// - X-axis: horizontal
/// - Y-axis: vertical
/// - Each position(x,y) contains a Cell
/// - Each cell can contain: block, tower, monkey, or be empty
/// 
/// Example grid (5x4):
/// Y=4 [P][B][B][ ][ ]...[ ][E]  <- Player Ship (left) → Blocks → Empty → Enemy Spawn (right)
/// Y=3 [P][B][B][ ][ ]...[ ][E]
/// Y=2 [P][B][B][ ][ ]...[ ][E]
/// Y=1 [P][B][B][ ][ ]...[ ][E]
/// Y=0 [P][B][B][ ][ ]...[ ][E]
///      X=0 X=1 X=2 X=3 ... X=n
/// 
/// Lane: a row of cells from Player base to Enemy base, the number of lanes is Grid's height
/// Layout Flow: Player Ship (left) → Blocks (Defense) → Empty Area → Enemy Spawn (right)
/// Each row represents a lane where entities move horizontally
/// </summary>
public class GridSystem : IGrid {
    public static new GridSystem Ins { get; private set; }

    [Header("Prefabs")]
    public ICell cellPrefab;   // Prefab for individual grid cells
    public GameObject targetMonkeyPrefab;
    public GameObject targetAllenPrefab;
    protected override void Awake(){
        base.Awake();
        Ins = this;
    }
    public override void Clear(){
        foreach(Transform child in transform){
            DestroyImmediate(child.gameObject);
        }
    }
    /// <summary>
    /// Hàm này chỉ tạo cell trống, không thực sự tạo battlefield bài bản cho từng level
    /// </summary>
    public override void Initialize(int width, bool[] openLanes)
    {
        Ins = this;
        this.width = width;
        height = openLanes.Length;
        cells = new Cell[width, height];
        this.openLanes = (bool[])openLanes.Clone();
        //Bounds
        bounds = new Bound(0, GridToWorldPosition(0, height - 1).y, 0, GridToWorldPosition(width - 1, 0).x);
        //Create cell for entire 2D array 
        for(int i = 0; i < width; ++i) {
            for(int j = 0; j < height; ++j) {
                CreateCell(cellPrefab, i, j);
            }
        }
        SlidingCamera.Ins.Initialize(bounds.right, bounds.left);
        Debug.Log("Successfully initialize Cells");
    }

    public override void CreateCell(ICell cellPrefab, int x, int y)
    {
        Vector3 worldPosition = GridToWorldPosition(x, y);
        Cell cell = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform).GetComponent<Cell>();
        cell.Initialize(x, y);
        cells[x, y] = cell;
    }
    public override Vector2 GridToWorldPosition(Vector2 gridPos)
    {
        return GridToWorldPosition(gridPos.x, gridPos.y);
    }
    public override Vector2 GridToWorldPosition(float x, float y){
        return new Vector2(x * CELL_SIZE, y * CELL_SIZE);
    }
    /// <summary>
    /// Converts Unity world coordinates to grid coordinates. WorldToGridPosition(new Vector3(2.7f, 3.2f, 0)) returns Vector2Int(2, 3)
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
    /// <example>IsValidGridPosition(5, 3) returns false for a 5x4 grid</example>
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
        List<int> openLanes = new();
        for(int i = 0; i < GridSystem.Ins.openLanes.Length; ++i) {
            if(GridSystem.Ins.openLanes[i]) {
                openLanes.Add(i);
            }
        }
        return openLanes;
    }
}




