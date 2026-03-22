using System;
using UnityEngine;

public class BuildBehaviour : IBehaviour, IOnApply {
    public enum State {
        ToConstructionSite,
        Building,
        ToHome,
    }
    public State state = State.ToConstructionSite;
    public bool isSecondBuilder = false;
    public Vector2Int dest;
    public int diff;
    public UnfinishedTower unfinishedTower;
    public void Initialize(UnfinishedTower unfinishedTower, bool isSecondBuilder) {
        if(isSecondBuilder) {
            diff = e.team == Team.Player ? -1 : 1;
        }
        else {
            diff = e.team == Team.Player ? 1 : -1;
        }
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
    public override void UpdateBehaviour() {
        if(state == State.ToConstructionSite) {
            if(Math.Abs(e.gridPos.x - (dest.x + diff)) <= 0.1f) {
                state = State.Building;
                e.model.PlayAnimation("Skill 1");
                e.model.Event.OnAnimationFinished += (AnimatorStateInfo info) => {
                    if(info.IsName("Skill 1")) {
                        unfinishedTower.AddProgress();
                    }
                };
                e.GetEffectable().ApplyEffect(new StunImmunity());
            }
        }
        else if(state == State.Building) {
            
        }
        else {
            
        }
    }
    public static void CreateBuilder(EntitySO tower, Vector2Int dest, Team team, int level) {
        int startX = team == Team.Player ? 0 : GridSystem.Ins.width - 1;
        BuildBehaviour newBuilder = EContainer.Ins.CreateEntity(
            SingletonRegister.Get<PrefabRegisterSO>().builder, startX, dest.y, team).GetComponent<BuildBehaviour>();
        BuildBehaviour newBuilder_2 = EContainer.Ins.CreateEntity(
            SingletonRegister.Get<PrefabRegisterSO>().builder, startX, dest.y, team).GetComponent<BuildBehaviour>();
        UnfinishedTower unfinishedTower = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().unfinishedTower).GetComponent<UnfinishedTower>();
        unfinishedTower.Initialize(new EntitySetting { so = tower, lane = dest.y, x = dest.x, level = level }
            , newBuilder, newBuilder_2);
        newBuilder.Initialize(unfinishedTower, false);
        newBuilder_2.Initialize(unfinishedTower, true);
    }
    public void ReturnToHome() {
        state = State.ToHome;
    }
    public override int GetPriority() => 0;
}