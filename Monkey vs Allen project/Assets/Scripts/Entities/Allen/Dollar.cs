using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dollar : MonoBehaviour{
    int lane;
    bool isFalling = true;
    Vector2 destination;
    public void Initialize(Vector2Int gridPos){
        lane = gridPos.y;
        destination = GridSystem.Ins.GridToWorldPosition(gridPos);
    }
    void Update(){
        if (isFalling){
            return;
        }
        foreach(Entity e in EContainer.Ins.GetEntitiesByLane(lane)){
            if (Mathf.Abs(e.GetWorldPosition().x - destination.x) <= 0.5f){
                e.effectController.ApplyEffect(new Greedy(e));
                Destroy(this.gameObject);
            }
        }
    }
    private class Greedy : IEffect, IStackable{
        int cnt = 1;
        public Greedy(Entity target) : base(target){
        }
        public void Stack(int amount){
            cnt += amount;
            if (cnt == 3){
                owner.GetEffectController().ApplyEffect(new Hypnotized(owner, -1));
            }
        }
    }
}