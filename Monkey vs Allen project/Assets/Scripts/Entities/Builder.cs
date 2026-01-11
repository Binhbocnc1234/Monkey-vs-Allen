using System;
using UnityEngine;

public class Builder : IBehaviour {
    private Vector2Int dest;
    private int diff;
    private EntitySO tower;
    private Timer finishTimer;
    private State state;
    private enum State {
        Running,
        Building,
    }
    public override void Initialize() {
        base.Initialize();
        dangerPoint = 15;
        finishTimer = new Timer(10, reset: true);
    }
    public static void CreateBuilder(EntitySO tower, Vector2Int dest, Team team) {
        int startX = team == Team.Player ? 0 : GridSystem.Ins.width - 1;
        Builder newBuilder = EContainer.Ins.CreateEntity(
            PrefabRegister.Ins.builder, new Vector2Int(startX, dest.y), team).GetComponent<Builder>();
        Builder newBuilder_2 = EContainer.Ins.CreateEntity(
            PrefabRegister.Ins.builder, new Vector2Int(startX, dest.y), team).GetComponent<Builder>();
        newBuilder.dest = dest;
        newBuilder.tower = tower;
        newBuilder.diff = team == Team.Player ? 1 : -1;
        newBuilder_2.dest = dest;
        newBuilder_2.tower = null;
        newBuilder.diff = team == Team.Player ? -1 : 1;
    }
    protected override void UpdateBehaviour() {
        if(state == State.Running && Math.Abs(e.GetWorldPosition().x - (dest.x + diff)) <= 0.1f) {
            state = State.Building;
            e.GetComponent<Move>().SetBehaviourEnable(false);
            e.animator.Play("Building");
            if(tower != null) {
                Vector3 oldScale = e.model.localScale;
                oldScale.y *= -1;
                e.model.localScale = oldScale;
            }
        }
        else if (state == State.Building) {
            if(finishTimer.Count()) {
                if(tower != null) {
                    EContainer.Ins.CreateEntity(tower, dest, e.team);
                }
                e.Die();
            }
        }
    }
}