using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WalkSynchronization : Move {
    public float strideLength; // in world unit
    public AnimationClip walkClip;
    private float originalSpeed;
    public override void Initialize() {
        base.Initialize();
        originalSpeed = walkClip.length / strideLength;
    }
    public override void UpdateBehaviour() {
        e.model.animator.SetFloat("MoveSpeed", e.GetRealMoveSpeed() / originalSpeed / IGrid.CELL_SIZE);
    }
}

