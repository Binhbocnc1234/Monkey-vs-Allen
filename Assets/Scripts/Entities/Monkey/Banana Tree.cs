using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BananaTree : IBehaviour, IOnApply {
    public enum State {
        Growing,
        Generating,
    }
    [NonSerialized] public State state = State.Growing;
    [NonSerialized] public int bananaCount;
    public event Action<State> OnStateChanged;
    public event Action OnBananaGenerated;
    [NonSerialized] public Timer cooldownTimer, shakingTimer;
    [HideInInspector] public float growProgress;
    public void OnApply() {
        ChangeState(State.Growing);
        if(cooldownTimer == null) {
            cooldownTimer = new Timer(BattleInfo.resourceDelay, true);
            cooldownTimer.SetCurTime(UnityEngine.Random.Range(BattleInfo.resourceDelay/2 - 3, BattleInfo.resourceDelay/2 + 3));
            shakingTimer = new Timer(1, true);
            bananaCount = BattleInfo.resourcePerGeneration;
        }
    }
    public override bool CanActive() {
        return true;
    }
    public override void UpdateBehaviour(float deltaTime) {
        if(state == State.Growing) { //Growing
            growProgress = cooldownTimer.GetPercent();
            if(cooldownTimer.Count(deltaTime)) {
                ChangeState(State.Generating);
            }
        }
        else if(state == State.Generating) {
            if(shakingTimer.Count(deltaTime)) {
                CreateBananas();
                ChangeState(State.Growing);
            }
        }
    }
    void ChangeState(State state) {
        this.state = state;
        OnStateChanged?.Invoke(state);
    }
    public void CreateBananas() {
        if(e.isSimulated) {
            BattleInfo.teamDict[e.team].resource += bananaCount;
        }
        else {
            OnBananaGenerated?.Invoke();
        }
    }
    public override int GetPriority() => 1;
    public override string GetAnimatorStateName() => "Shake";
    public override List<APModifier> GetAssessPoint() {
        return new() { new APModifier(Operator.Addition, APType.NeedProtection, 50) };
    }
}
