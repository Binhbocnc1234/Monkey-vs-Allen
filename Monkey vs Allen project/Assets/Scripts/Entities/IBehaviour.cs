using System;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class IBehaviour : MonoBehaviour{
    private bool isEnable = true;
    protected Entity e;
    [ReadOnly] public int dangerPoint = 0;
    public virtual void Initialize() {
        e = GetComponent<Entity>();
        e.OnStateChanged += OnStateChange;
    }
    public virtual void OnStateChange(EntityState state){

    }
    void Update(){
        if (isEnable && BattleInfo.gameState == GameState.Fighting){
            UpdateBehaviour();
        }
    }
    protected abstract void UpdateBehaviour();

    public void SetBehaviourEnable(bool toggle){
        isEnable = toggle;
    }

}