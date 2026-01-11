using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public partial class LevelInitializer : Singleton<LevelInitializer> {
    public EntitySO targetMonkeySO, targetAllenSO;
    Dictionary<GridInitializerEnum, Action<LevelSO, GridSystem>> actions;

    protected override void Awake() {
        base.Awake();
        actions = new() {
            {GridInitializerEnum.Level_1, Level_1_Garden_Init},
            {GridInitializerEnum.Garden , GardenInit},
            {GridInitializerEnum.Level_2, Level_2_Garden_Init},
        };
    }
    public void Initialize(LevelSO so, GridSystem grid){
        actions[so.initializer](so, grid);
        InitTarget(grid);
    }
    void InitTarget(GridSystem grid){
        for(int y = 0; y < IGrid.ARRAY_HEIGHT; ++y){
            if (grid.openLanes[y] == false) continue;
            EContainer.Ins.CreateEntity(targetMonkeySO, new Vector2Int(1, y), Team.Player);
            EContainer.Ins.CreateEntity(targetAllenSO, new Vector2Int(IGrid.Ins.width - 2, y), Team.Enemy);
        }
    }

}
