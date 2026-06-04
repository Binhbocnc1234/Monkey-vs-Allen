using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
/// <summary>
/// Defines a runtime behaviour template that can be cloned, bound to an entity, and updated during battle <br/>
/// </summary>
public abstract class IBehaviour : IAssessable {
    private bool _isEnable = true;
    public bool isEnable {
        get { return _isEnable; }
        set { _isEnable = value; }
    }
    protected IEntity e { get; private set; }
    /// <summary>
    /// Binds this behaviour instance to the entity that owns and drives it <br/>
    /// </summary>
    public virtual void SetEntity(IEntity entity) { e = entity; }
    /// <summary>
    /// Creates a shallow clone so public template data can be reused per entity instance <br/>
    /// </summary>
    public IBehaviour GetClone() => (IBehaviour)this.MemberwiseClone();
    public virtual void UpdateBehaviour(float deltaTime) { } // Called by Entity.cs
    public abstract bool CanActive(); // Called by Entity.cs to check if this behaviour can be used now
    public virtual List<APModifier> GetAssessPoint() {
        return new();
    }
    public abstract int GetPriority(); // Liệu một Behaviour khác có khả năng chiếm quyền kiểm soát
    public abstract string GetAnimatorStateName();
}
