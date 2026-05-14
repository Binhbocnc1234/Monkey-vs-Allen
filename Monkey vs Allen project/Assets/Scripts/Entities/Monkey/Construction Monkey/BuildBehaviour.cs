using System;
using UnityEngine;

public class BuildBehaviour : IBehaviour {
    public bool isSecondBuilder = false;
    public Vector2 dest;
    public int diff;
    public UnfinishedTower unfinishedTower;
    public Timer buildDelay;
    public void Initialize(UnfinishedTower unfinishedTower, bool isSecondBuilder) {
        if(isSecondBuilder) {
            diff = e.team == Team.Left ? 1 : -1;
        }
        else {
            diff = e.team == Team.Left ? -1 : 1;
        }
        this.dest = unfinishedTower.transform.position / IGrid.CELL_SIZE;
        buildDelay = new Timer(1, reset: true);
        e.GetEffectable().ApplyEffect(new StunImmunity());
    }
    public override void UpdateBehaviour(float deltaTime) {
        if(buildDelay.Count()) {
            if(unfinishedTower.AddProgress()) {
                e.Die();
            }
            else {
                e.ReturnToIdleBehaviour();
            }
        }
    }
    public override string GetAnimatorStateName() => "Skill 1";
    public override bool CanActive() {
        return Math.Abs(e.gridPos.x - (dest.x + diff)) <= 0.1f;
    }
    public override int GetPriority() => 2;
}