using System;
using System.Collections.Generic;
using UnityEngine;

/// Chỉ có một IBehaviour enable mỗi 
[RequireComponent(typeof(IEntity))]
public abstract class IBehaviour : EntityBehaviour, IAssessable {
    [ReadOnly] public bool canActive;
    public bool isEnable {
        get { return enabled; }
        set { enabled = value; }
    }
    void Update() {
        canActive = CanActive();
    }
    public virtual void UpdateBehaviour(float deltaTime) { } // Called by Entity.cs
    public abstract bool CanActive(); // Called by Entity.cs to check if this behaviour can be used now
    public void ApplyData(BehaviourData skillData) {

    }
    public virtual List<APModifier> GetAssessPoint() {
        return new();
    }
    public abstract int GetPriority(); // Liệu một Behaviour khác có khả năng chiếm quyền kiểm soát 
    public abstract string GetAnimatorStateName();
}

public abstract class BehaviourData {
    public string id = "noId";
    public float remainingTime;
}
public class YornInfinityArrowData : BehaviourData {
    public int shotIndex;
}
public class MeleeAttackData : BehaviourData {
    
}