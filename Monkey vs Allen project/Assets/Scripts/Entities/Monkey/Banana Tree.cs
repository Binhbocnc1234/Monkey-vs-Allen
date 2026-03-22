using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Playables;


//Những cây sau sẽ có giá tiền tăng thêm 1
public class BananaTree : IBehaviour
{
    public enum State {
        Growing,
        Generating,
    }
    public State state = State.Growing;
    public Banana bananaPrefab;
    public SpriteRenderer bananaRenderer;
    public int bananaCount = 3;
    public Action<State> OnStateChanged;
    public Timer cooldownTimer, shakingTimer;
    public EventChannel channel = new();
    void Awake() {
        cooldownTimer = new Timer(7, true);
        shakingTimer = new Timer(1, true);
    }
    public override void Initialize() {
        base.Initialize();
        ChangeState(State.Growing);
    }
    public override bool CanActive() {
        return true;
    }
    public override void UpdateBehaviour() {
        if(state == State.Growing) { //Growing
            bananaRenderer.transform.localScale = Vector3.one * cooldownTimer.GetPercent();
            if(cooldownTimer.Count()) {
                ChangeState(State.Generating);
            }
        }
        else if (state == State.Generating) {
            if(shakingTimer.Count()) {
                CreateBananas();
                ChangeState(State.Growing);
            }
        }
    }
    void ChangeState(State state) {
        this.state = state;
        OnStateChanged?.Invoke(state);
        if(state == State.Growing) {
            e.model.PlayAnimation("Idle");
        }
        else if (state == State.Generating) {
            e.model.PlayAnimation("Shake");
        }
    }
    public void CreateBananas() {
        Banana bananaBunch = GeneralPurposeContainer.Ins.CreateInstance(SingletonRegister.Get<PrefabRegisterSO>().bananaBunch.GetComponent<Banana>(), transform.position);
        bananaBunch.Initialize(e.lane);
        bananaBunch.SetBananaCount(bananaCount);
    }
    public override int GetPriority() => 1;
}
