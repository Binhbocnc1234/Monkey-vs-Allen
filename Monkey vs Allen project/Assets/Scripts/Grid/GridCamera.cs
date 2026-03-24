using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridCamera : MyCamera
{
    public static new GridCamera Ins { get; private set; }
    private GridSystem grid;
    protected new void Awake() {
        Ins = this;
    }
    protected override void Start() {
        base.Start();
        grid = GridSystem.Ins;
    }
    public void MoveTowardPlayerHouse() {
        SetTarget(new Vector3(3, 5, -10));
    }
    public void MoveTowardEnemyHouse() {
        SetTarget(new Vector3(grid.GetCell(grid.width-1, 0).transform.position.x, 5, -10));
    }
}

