using System;
using UnityEngine;

public class BuildBehaviour : IBehaviour, IOnApply {
    public bool isSecondBuilder = false;
    public Vector2 dest;
    public int diff;
    public UnfinishedTower unfinishedTower;
    public void Initialize(UnfinishedTower unfinishedTower, bool isSecondBuilder) {
        if(isSecondBuilder) {
            diff = e.team == Team.Player ? 1 : -1;
        }
        else {
            diff = e.team == Team.Player ? -1 : 1;
        }
        this.dest = unfinishedTower.transform.position/IGrid.CELL_SIZE;
        e.model.Event.OnAnimationFinished += (AnimatorStateInfo info) => {
            if(info.IsName("Skill 1")) {
                if(unfinishedTower.AddProgress()) {
                    e.Die();
                }
                else {
                    e.ReturnToIdleBehaviour();
                }
            }
        };
        e.GetEffectable().ApplyEffect(new StunImmunity());
    }
    public void OnApply() {
        e.model.PlayAnimation("Skill 1");
        if (isSecondBuilder) {
            // There is two entity facing to build position, opposite to each other
            e.model.GetComponent<Rotater>().FlipX();
        }
    }
    public override bool CanActive() {
        return Math.Abs(e.gridPos.x - (dest.x + diff)) <= 0.1f;
    }
    public override int GetPriority() => 2;
}