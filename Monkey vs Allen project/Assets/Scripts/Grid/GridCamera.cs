using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridCamera : MyCamera
{
    private static GridCamera _Ins;
    public static new GridCamera Ins {
        get {
            if (_Ins == null)
            {
                _Ins = FindObjectOfType<GridCamera>();
                if (_Ins == null){
                    Debug.LogError($"Not found singleton of {typeof(GridCamera)}");
                }
            }
            return _Ins;
        }
    }
    private IGrid grid;
    protected override void Awake() {
        base.Awake();
        grid = IGrid.Ins;
    }
    public void MoveTowardPlayerHouse() {
        SetTarget(new Vector3(3, 5, -10));
    }
    public void MoveTowardEnemyHouse() {
        SetTarget(new Vector3(grid.GetCell(grid.width-1, 0).transform.position.x, 5, -10));
    }
}

