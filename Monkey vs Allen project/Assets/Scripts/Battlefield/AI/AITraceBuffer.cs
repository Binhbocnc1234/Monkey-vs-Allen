using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Trace {
    /// <summary>
    /// This field is written by EnemyDecisionTraceBuffer
    /// </summary>
    public float timeStamp;
    public float actionScore;
    public int resourceBefore;
    public int resourceAfter;
    public int cost;
}
[System.Serializable]
public class UpgradeTrace : Trace {
    public int upgradeNumber;
}
[System.Serializable]
public class UseCardTrace : Trace {
    public List<string> cardName;
    public int lane;
}
[System.Serializable]
public class WaitTrace : Trace {
    public string futureScore;
    public float bestLookahead;
}
public class AITraceBuffer : MonoBehaviour {
    public int maxCount;
    [SubclassSelector, SerializeReference] public List<Trace> traceList;

    public void Push(Trace trace) {
        if(traceList == null) {
            return;
        }
        trace.timeStamp = BattleInfo.timeElapsed;
        traceList.Add(trace);
        while(traceList.Count > maxCount) {
            traceList.RemoveAt(0);
        }
    }
}
