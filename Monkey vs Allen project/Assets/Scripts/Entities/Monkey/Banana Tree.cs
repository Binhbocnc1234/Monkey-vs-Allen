using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


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
    public int bananaCount = 4;
    public Action<State> OnStateChanged;
    public Timer cooldownTimer, shakingTimer;
    public EventChannel channel = new();
    public override void Initialize() {
        base.Initialize();
        cooldownTimer = new Timer(7, true);
        shakingTimer = new Timer(1, true);
        dangerPoint = 1;
        ChangeState(State.Growing);
    }
    
    protected override void UpdateBehaviour() {
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
            e.animator.Play("Idle");
        }
        else if (state == State.Generating) {
            e.animator.Play("Shake");
        }
    }
    public void CreateBananas(){
        Banana bananaBunch = GeneralPurposeContainer.Ins.CreateInstance(SingletonRegister.Get<PrefabRegisterSO>().bananaBunch.GetComponent<Banana>(), transform.position);
        bananaBunch.Initialize(e.laneIndex);
        bananaBunch.SetBananaCount(bananaCount);
    }
}
