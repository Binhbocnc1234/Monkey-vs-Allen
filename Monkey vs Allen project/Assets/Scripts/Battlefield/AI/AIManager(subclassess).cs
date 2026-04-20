using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LaneAssessmentDetail {
    public int lane;
    public IEntity leader;
    public IEntity enemyLeader;

    // Player-side snapshot/forecast metrics in this lane.
    public float playerDanger = 0, playerSurvivability = 0, playerPower = 0;
    // Enemy-side snapshot/forecast metrics in this lane.
    public float enemyDanger = 0, enemySurvivability = 0, enemyPower = 0;

    // Backward-compatible aliases used by existing scoring code.
    public float danger = 0, survivability = 0;
    // Count of Player units that contribute to lane pressure.
    public int opponentCount = 0;
    // Raw weighted NeedProtection sum, reused by global urgency.
    public float totalNeedProtection = 0;
    // Source of this penalty: balance heuristic for formation fragility.
    // With equal total danger/defend, many small units are weaker than one consolidated unit
    // because a single death drops the total lane power quickly.
    public float power = 0;

    public void SyncLegacyFieldsFromPlayerMetrics() {
        danger = playerDanger;
        survivability = playerSurvivability;
        power = playerPower;
    }
}

public abstract class AIAction{
    public float score = 0;
    public float cost = 0;
    public float lookahead = 0f;
    public event Action OnExecute;
    public void ExecuteAndNotify(){
        Execute();
        OnExecute?.Invoke();
    }
    protected abstract void Execute();
}

public class BundleDecision : AIAction {
    public List<IBattleCard> actions = new();
    public int lane;
    public void CopyFrom(List<IBattleCard> src, float score) {
        actions.Clear();
        actions.AddRange(src);
        this.score = score;
        this.cost = 0;
        foreach(var cardAction in actions) {
            this.cost += cardAction.cost;
        }
    }
    protected override void Execute() {
        foreach(var battleCard in actions) {
            battleCard.UseCard(new Vector2Int(-1, lane));
        }
    }
}
public class UpgradeAction : AIAction{
    protected override void Execute() {
        AlienResourceManager.Ins.Upgrade();
    }
}