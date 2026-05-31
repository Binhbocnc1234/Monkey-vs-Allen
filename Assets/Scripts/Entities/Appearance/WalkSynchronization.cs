using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WalkSynchronization : EntityAppearance {
    public float strideLength; // in world unit
    public AnimationClip walkClip;
    private float originalSpeed;
    public override void Initialize(EntityModel model) {
        base.Initialize(model);
        originalSpeed = strideLength / walkClip.length;
    }
    void Update() {
        GetComponent<EntityModel>().animator.SetFloat("MoveSpeed", Move.GetUnityMoveSpeed(e) / originalSpeed);
    }
}

