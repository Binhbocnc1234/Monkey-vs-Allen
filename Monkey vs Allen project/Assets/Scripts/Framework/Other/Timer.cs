using UnityEngine;
using System;

[Serializable]
public class Timer {
    public float remainingTime;
    public float totalTime;
    public bool isEnd = false;
    public bool isReset = true;
    public Timer(float totalTime, bool reset) {
        this.totalTime = totalTime;
        isReset = reset;
        Reset();
    }
    public bool Count() {
        SetCurTime(remainingTime - Time.deltaTime);
        return isEnd;
    }
    public void SetCurTime(float amount) {
        remainingTime = amount;
        if(remainingTime <= 0) {
            if(isReset) {
                remainingTime = totalTime;
            }
            else {
                remainingTime = 0;
            }
            isEnd = true;
        }
        else {
            isEnd = false;
        }
    }
    public void FinishCooldown() {
        SetCurTime(0);
    }
    public void Reset() {
        remainingTime = totalTime;
        isEnd = false;
    }
    public float GetPercent() => 1 - remainingTime / totalTime;
}