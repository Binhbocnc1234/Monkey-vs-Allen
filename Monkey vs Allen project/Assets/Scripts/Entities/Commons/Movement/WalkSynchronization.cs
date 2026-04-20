using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WalkSynchronization : Move, IInitialize {
    public float strideLength; // in world unit
    public AnimationClip walkClip;
    private float originalSpeed;
    public void Initialize() {
        originalSpeed = strideLength / walkClip.length;
    }
    public override void UpdateBehaviour() {
        base.UpdateBehaviour();
        e.model.animator.SetFloat("MoveSpeed", e.GetRealMoveSpeed() / originalSpeed);
    }
}

