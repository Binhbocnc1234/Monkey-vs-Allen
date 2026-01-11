using UnityEngine;
using System.Collections.Generic;


public abstract class IBattleCard : IUpdatePerFrame {
    public enum SelectMessage {
        InsuffientBanana,
        Recovering,
        CanSelect
    }
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
    public virtual float GetCooldownPercent() {
        return cooldownTimer.GetCurTime() / cooldownTimer.totalTime;
    }
    public bool IsFinishedCooldown() {
        return cooldownTimer.isEnd;
    }
    public virtual void SetCoolDown(float newCoolDown) {
        cooldownTimer = new Timer(newCoolDown, false);
    }
    public float GetCoolDown() => cooldownTimer.totalTime;
    public virtual CardSO GetSO() {
        return so;
    }
    public int GetCost() {
        return cost;
    }
}
