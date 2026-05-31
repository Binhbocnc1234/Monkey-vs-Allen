using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public abstract class IBehaviour : IAssessable {
    private bool _isEnable = true;
    public bool isEnable {
        get { return _isEnable; }
        set { _isEnable = value; }
    }
    protected IEntity e { get; private set; }
    public void SetEntity(IEntity entity) { e = entity; }
    public IBehaviour GetClone() => (IBehaviour)this.MemberwiseClone();
    public virtual void UpdateBehaviour(float deltaTime) { } // Called by Entity.cs
    public abstract bool CanActive(); // Called by Entity.cs to check if this behaviour can be used now
    public virtual List<APModifier> GetAssessPoint() {
        return new();
    }
    public abstract int GetPriority(); // Liệu một Behaviour khác có khả năng chiếm quyền kiểm soát
    public abstract string GetAnimatorStateName();
}
