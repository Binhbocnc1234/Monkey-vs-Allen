using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


//Những cây sau sẽ có giá tiền tăng thêm 1
public class BananaTree : IBehaviour
{
    public enum State {
        Growing,
        ReadyToCollect,
    }
    public State state = State.Growing;
    public Banana bananaPrefab;
    public Sprite fineBanana, rawBanana;
    public SpriteRenderer bananaRenderer;
    public float cooldown = 7f;
    public int stack = 0;
    public int maxStack = 4;
    public Action<BananaGenerated> OnBananaGenerated;
    public Timer cooldownTimer;
    public EventChannel channel = new();
    public override void Initialize(){
        base.Initialize();
        cooldownTimer = new Timer(cooldown, false);
        dangerPoint = 1;
        ((TreeEvent)e.animatorEvent).OnShake += () => {
            if (state == State.ReadyToCollect){
                state = State.Growing;
                cooldownTimer.Reset();
                CreateBananas();
            }
        };
    }
    protected override void UpdateBehaviour(){
        if (cooldownTimer.Count()){
            state = State.ReadyToCollect;
            if (stack < maxStack){
                stack++;
                cooldownTimer.Reset();
            }
            bananaRenderer.sprite = fineBanana;
        }
        else{ //Growing
            bananaRenderer.transform.localScale = Vector3.one * cooldownTimer.GetPercent();
            bananaRenderer.sprite = rawBanana;

        }
    }
    public void CreateBananas(){
        OnBananaGenerated?.Invoke(new BananaGenerated(this, stack));
        channel.Invoke(new BananaGenerated(this, stack));
        while(stack > 0) {
            stack--;
            // Tạo nải chuối rơi ra từ mặt đất
            Instantiate(bananaPrefab, transform.position, Quaternion.identity, this.transform).Initialize(e.laneIndex, 0.5f);
        }
        
    }
    void OnMouseDown(){
        e.animator.Play("Shake");
        int randomInt = UnityEngine.Random.Range(0, 2);
        if (randomInt == 1 && cooldownTimer.isEnd){
            CreateBananas();
        }
    }
    public class BananaGenerated : MyEvent<BananaTree>{
        int count;
        public BananaGenerated(BananaTree caller, int count) : base(caller) {
            this.count = count;
        }
        public int GetCount(){
            return count;
        }
    }
}
