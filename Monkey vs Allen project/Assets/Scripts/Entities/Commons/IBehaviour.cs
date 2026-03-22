using System;
using UnityEngine;

/// Chỉ có một IBehaviour enable mỗi 
[RequireComponent(typeof(Entity))]
public abstract class IBehaviour : MonoBehaviour{
    public bool isEnable {
        get { return enabled; }
        set{ enabled = value; }
    }
    protected Entity e;
    public virtual void Initialize() {
        e = GetComponent<Entity>();
    }
    void Update() {}
    public virtual void UpdateBehaviour(){} // Called by Entity.cs
    public abstract bool CanActive(); // Called by Entity.cs to check if this behaviour can be used now
    public void ApplyData(BehaviourData skillData) {

    }
    public virtual int GetDangerPoint() {
        return 0;
    }
    public abstract int GetPriority(); // Liệu một Behaviour khác có khả năng chiếm quyền kiểm soát 
}