using UnityEngine;
using System;

public class Timer {
    private float curTime = 0;
    public float totalTime;
    public bool isEnd = false;
    public bool isReset = true;
    public Timer(float totalTime, bool reset) {
        this.totalTime = totalTime;
        isReset = reset;
    }
    public bool Count() {
        SetCurTime(curTime + Time.deltaTime);
        return isEnd;
    }
    public void SetCurTime(float amount) {
        curTime = amount;
        if(curTime >= totalTime) {
            if(isReset) {
                curTime = 0;
            }
            else {
                curTime = totalTime;
            }
            isEnd = true;
        }
        else {
            isEnd = false;
        }
    }
    public void FinishCooldown() {
        SetCurTime(totalTime);
    }
    public float GetCurTime() => curTime;
    public float GetPercent() => curTime / totalTime;
    public void Reset() {
        curTime = 0;
        isEnd = false;
    }
}