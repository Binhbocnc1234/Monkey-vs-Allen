using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeamSnapshot {
    public float attackFocusPoint = float.NaN;
    public float defensiveFocusPoint = float.NaN;
    public float danger = 0;
    public float survivability = 0;
    public int unitCount = 0;
    public float totalNeedProtection = 0;
    public float avgMoveSpeed = 0;
    private float _power = -1;
    public float power {
        get {
            if(_power == -1) {
                _power = danger * survivability * GetUnitCountDebuff();
            }
            return _power;
        }
        set {
            _power = value;
        }
    }
    public static float GetUnitCountDebuff(int specifiedUnitcount) => Mathf.Max(1.1f - 0.1f * specifiedUnitcount, 0.5f);
    public float GetUnitCountDebuff() => Mathf.Max(1.1f - 0.1f * unitCount, 0.5f);
}

[System.Serializable]
public class LaneAssessment {
    public int lane;
    public float lookAhead;
    public float firstTimeToContact = float.PositiveInfinity, contactPos = float.NaN;
    public TeamSnapshot leftSide = new(), rightSide = new();
    public TeamSnapshot this[Team team] {
        get {
            return team == Team.Left ? leftSide : rightSide;
        }
        set {
            if(team == Team.Left) leftSide = value;
            else { rightSide = value; }
        }
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
    public List<IBattleCard> usedCards = new();
    public int lane;
    public void CopyFrom(List<IBattleCard> src, float score) {
        usedCards.Clear();
        usedCards.AddRange(src);
        this.score = score;
        this.cost = 0;
        foreach(var cardAction in usedCards) {
            this.cost += cardAction.cost;
        }
    }
    protected override void Execute() {
        foreach(var battleCard in usedCards) {
            battleCard.UseCard(new Vector2Int(-1, lane));
        }
    }
}
public class UpgradeAction : AIAction{
    protected override void Execute() {
        AlienResourceManager.Ins.Upgrade();
    }
}