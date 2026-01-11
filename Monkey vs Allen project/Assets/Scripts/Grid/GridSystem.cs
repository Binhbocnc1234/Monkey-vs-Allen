using System.Collections.Generic;
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
    public GameObject cellPrefab;   // Prefab for individual grid cells
    public GameObject targetMonkeyPrefab;
    public GameObject targetAllenPrefab;
    public Cell[,] cells { get;  private set;}      // 2D array storing all grid cells
    public int[] headerPosition; //header: lead block for each lane, position follow X-asis
    protected override void Awake(){
        base.Awake();
        Ins = this;
    }
    public override void Clear(){
        foreach(Transform child in transform){
            DestroyImmediate(child.gameObject);
        }
    }

    public override void Initialize(LevelSO levelSO)
    {
        Ins = this;
        this.width = levelSO.gridWidth;
        this.height = levelSO.gridHeight;
        cells = new Cell[width, ARRAY_HEIGHT];
        // headerPosition = new int[];
        transform.position = new Vector2(0, 0); //Usually (0, 0) in world space

        //Bounds
        bounds = new GridBound();
        bounds.left = 0; bounds.bottom = 0;
        bounds.top = GridToWorldPosition(0, height - 1).y;
        bounds.right = GridToWorldPosition(width - 1, 0).x;
        
        //Create cell for entire 2D array
        for (int x = 0; x < width; x++){
            for (int y = 0; y < ARRAY_HEIGHT; y++)
            {
                CreateCell(x, y);
            }
        }
        Debug.Log("Successfully initialize Cells");
    }

    void CreateCell(int x, int y)
    {
        Vector3 worldPosition = GridToWorldPosition(x, y);
        GameObject cellObject = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform);
        
        Cell cell = cellObject.GetComponent<Cell>();
        cell.gridPosition = new Vector2Int(x, y);
        

        SortingGroup sortingGroup = cellObject.GetComponent<SortingGroup>();
        sortingGroup.sortingOrder = height - y;

        cell.name = $"Cell_{x}_{y}";
        cells[x, y] = cell;
    }
    public override Vector2 GridToWorldPosition(Vector2Int gridPos)
    {
        return GridToWorldPosition(gridPos.x, gridPos.y);
    }
    public override Vector2 GridToWorldPosition(int x, int y){
        return new Vector2(x * cellSize, y * cellSize);
    }
    /// <summary>
    /// Converts Unity world coordinates to grid coordinates. WorldToGridPosition(new Vector3(2.7f, 3.2f, 0)) returns Vector2Int(2, 3)
    /// </summary>
    public override Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        Vector2 localPosition = worldPosition;
        Vector2Int ans = new Vector2Int(
            Mathf.FloorToInt(localPosition.x / cellSize),
            Mathf.FloorToInt(localPosition.y / cellSize)
        );
        if (IsValidGridPosition(ans)){
            return ans;
        }
        else{
            return new Vector2Int(-1, -1);
        }
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
    public override bool IsValidGridPosition(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < ARRAY_HEIGHT;
    }
    
    /// <summary>
    /// Overloaded version that takes Vector2Int coordinates.
    /// </summary>
    public bool IsValidGridPosition(Vector2Int position)
    {
        return IsValidGridPosition(position.x, position.y);
    }

    public List<int> GetOpenLanes(){
        List<int> openLanes = new();
        for(int i = 0; i < GridSystem.Ins.openLanes.Length; ++i) {
            if(GridSystem.Ins.openLanes[i]) {
                openLanes.Add(i);
            }
        }
        return openLanes;
    }
    /// <summary>
    /// Extends a lane by placing a specified number of connected blocks.
    /// The placed blocks must be connected to the previous/left blocks in the lane.
    /// <br/>
    /// Example: PlaceBlockAtLane(2, 3) places 3 blocks in lane 2, starting from the current header position.
    /// </summary>
    // public override void PlaceBlockAtLane(int y, int count = 1)
    // {
    //     // Validate lane index
    //     if (y < 0 || y >= height || count < 1) Debug.LogWarning("Lane index not valid");

    //     // Start placing from the current header position for this lane
    //     int startX = headerPosition[y] + 1;
    //     int placed = 0;
    //     for (int x = startX; x < width && placed < count; x++)
    //     {
    //         GetCell(x, y).PlaceBlock();
    //         placed++;
    //     }
    // }
}




