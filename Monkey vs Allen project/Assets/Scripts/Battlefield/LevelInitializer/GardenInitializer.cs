using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class LevelInitializer
{
    public GameObject blockPrefab;
    public EntitySO bananaTreeSO;
    public Sprite[] glassBlockSprites;
    public Sprite[] dirtBlockSprites;
    public Sprite dickBlockSprite;
    void GeneralInit(LevelSO levelSO, GridSystem grid, List<int> closedLanes){
        for (int x = 0; x < grid.width; x++){
            for (int y = 0; y < IGrid.ARRAY_HEIGHT; y++)
            {
                if (closedLanes.Contains(y)){
                    grid.GetCell(x, y).PlaceBlock(dirtBlockSprites[
                        Random.Range(0, dirtBlockSprites.Length)]);
                }
                else{
                    grid.openLanes[y] = true;
                    grid.GetCell(x, y).PlaceBlock(glassBlockSprites[
                        Random.Range(0, glassBlockSprites.Length)]);
                }
                
            }
        }
    }
    void GardenInit(LevelSO levelSO, GridSystem grid){
        GeneralInit(levelSO, grid, new List<int>());
    }
    void Level_1_Garden_Init(LevelSO levelSO, GridSystem grid){
        GeneralInit(levelSO, grid, new List<int>() {0, 1, 3, 4});
        Entity newBananaTree = EContainer.Ins.CreateEntity(bananaTreeSO, new Vector2Int(2, 2), Team.Player);
        BananaTree com = newBananaTree.GetComponent<BananaTree>();
        com.SetBehaviourEnable(false);

        float middle = grid.GridToWorldPosition(0, 2).y;
        grid.bounds.top =  middle + grid.cellSize/2;
        grid.bounds.bottom = middle - grid.cellSize / 2;

    }
    void Level_2_Garden_Init(LevelSO levelSO, GridSystem grid){
        GeneralInit(levelSO, grid, new List<int>() {0, 4});
        grid.bounds.top =  grid.GridToWorldPosition(0, 3).y + grid.cellSize/2;
        grid.bounds.bottom = grid.GridToWorldPosition(0, 1).y - grid.cellSize / 2;
    }
}
