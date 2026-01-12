using System;
using UnityEngine;

public class AdvancedTimer : IUpdatePerFrame, IDestroyable {
    private float curTime = 0;
    public float totalTime;
    public bool isEnd = true;
    public bool destroyWhenTimeUp = true;
    private Action action = null;
    private bool isDead = false;
    public AdvancedTimer(float totalTime, bool destroyWhenTimeUp, Action actionWhenTimeUp) {
        this.totalTime = totalTime;
        this.destroyWhenTimeUp = destroyWhenTimeUp;
        this.action = actionWhenTimeUp;
    }
    public void Update() {
        SetCurTime(curTime + Time.deltaTime);
    }
    public void SetCurTime(float amount) {
        curTime = amount;
        if(curTime >= totalTime) {
            if(!isEnd) {
                isEnd = true;
                action.Invoke();
            }
            if(destroyWhenTimeUp) {
                DestroyThis();
            }
            else {
                curTime = totalTime;
            }
        }
        else {
            isEnd = false;
        }
    }
    public float GetCurTime() => curTime;
    public float GetPercent() => curTime / totalTime;
    public void Reset() {
        curTime = 0;
        isEnd = false;
    }
    public void DestroyThis() {
        isDead = true;
    }
    public bool IsDead() => isDead;
}