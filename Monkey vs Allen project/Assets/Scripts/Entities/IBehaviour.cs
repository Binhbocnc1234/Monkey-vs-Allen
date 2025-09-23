using System;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class IBehaviour : MonoBehaviour{
    protected Entity e;
    [ReadOnly] public int dangerPoint = 0;
    protected virtual void Awake(){
        this.enabled = false;
        e = GetComponent<Entity>();
        e.OnStateChanged += OnStateChange;
    }
    public virtual void Initialize() { this.enabled = true; }
    public virtual void OnStateChange(EntityState state){

    }

}