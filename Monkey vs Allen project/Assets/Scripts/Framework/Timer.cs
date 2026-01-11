using UnityEngine;
using System;

public class Timer {
    private float curTime = 0;
    public float totalTime;
    public bool isEnd = true;
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

// public class AdvancedTimer : IUpdatePerFrame, IDestroyable {
//     private float curTime = 0;
//     public float totalTime;
//     public bool isEnd = true;
//     public bool isAutoReset = true;
//     private Action action = null;
//     private bool isDead = false;
//     public AdvancedTimer(float totalTime, bool isAutoReset, Action actionWhenTimeUp) {
//         this.totalTime = totalTime;
//         this.isAutoReset = isAutoReset;
//         this.action = actionWhenTimeUp;
//     }
//     public AdvancedTimer(float totalTime, bool isAutoReset) {
//         this.totalTime = totalTime;
//         this.isAutoReset = isAutoReset;
//     }
//     public void Update() {
//         SetCurTime(curTime + Time.deltaTime);
//     }
//     public void SetCurTime(float amount) {
//         curTime = amount;
//         if(curTime >= totalTime) {
//             if(!isEnd) {
//                 isEnd = true;
//                 action.Invoke();
//             }
//             if(isAutoReset) {
//                 curTime = 0;
//             }
//             else {
//                 curTime = totalTime;
//             }
//         }
//         else {
//             isEnd = false;
//         }
//     }
//     public float GetCurTime() => curTime;
//     public float GetPercent() => curTime / totalTime;
//     public void Reset() {
//         curTime = 0;
//         isEnd = false;
//     }
//     public void DestroyThis() {
//         isDead = true;
//     }
//     public bool IsDead() => isDead;
// }