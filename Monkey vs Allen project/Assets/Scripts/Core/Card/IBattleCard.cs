using UnityEngine;
using System.Collections.Generic;

public enum SelectMessage {
    InsuffientBanana,
    Recovering,
    CanSelect
}
public abstract class IBattleCard : MonoBehaviour {
    
    // public static 
    public bool isActive = true;
    public int cost;
    public int originalCost;
    public Timer cooldownTimer;
    public List<CardEffect> effects;
    protected CardSO so;
    // public abstract IEffect ReceiveEffect(CardEffectType effectType);
    public abstract bool CanUseCard(Vector2Int gridPosition);
    public abstract void UseCard(Vector2Int gridPosition);
    public abstract void Update();
    public abstract SelectMessage CanSelectCard();
    public bool IsFinishedCooldown() {
        return cooldownTimer.isEnd;
    }
    public abstract void SetCoolDown(float newCoolDown);
    public float GetCoolDown() => cooldownTimer.totalTime;
    public virtual CardSO GetSO() {
        return so;
    }
    public int GetCost() {
        return cost;
    }
}
