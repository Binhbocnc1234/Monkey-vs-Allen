using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

//Card không nên là một thực thể
public class BattleCard : IBattleCard
{
    public BattleCardUI cardUI;
    public Team team;
    public int cardLevel;
    int maxStack = 1;
    int stack = 0;
    public void Initialize(CardSO so, Team team, BattleCardUI cardUI){
        Initialize(so, team);
        this.cardUI = cardUI;
        cardUI.OnClickEvent += (c) => {
            SelectMessage selectMessage = CanSelectCard();
            Debug.Log(selectMessage);
            if(selectMessage == SelectMessage.InsuffientBanana) {
                CostInsuffientAnimation.Instantiate(cardUI);
            }
            else if(selectMessage == SelectMessage.CanSelect) {
                PointerUI.Ins.SetHoldingCard(this);
            }
        };
    }
    public void Initialize(CardSO so, Team team) {
        if(so == null) {
            Debug.LogError("ApplyCardSO : so is null");
            return;
        }
        this.so = so;
        this.cost = so.cost;
        this.originalCost = so.cost;
        SetCoolDown(EnumConverter.Convert(so.cooldownType));
        this.team = team;
        if(so is EnemyCardSO enemyCardSO) {
            this.maxStack = enemyCardSO.maxStack;
        }
        cardLevel = PlayerData.GetCardDataById(so.id).level;
    }
    public override void Update() {
        if(isActive) {
            if(cooldownTimer.Count() && stack < maxStack) {
                stack++;
                if (stack < maxStack) {
                    cooldownTimer.Reset();
                }
            }
            if(cardUI != null) {
                cardUI.UpdateBattleCard(this);
            }
        }
    }

    public override void SetCoolDown(float newCoolDown) {
        cooldownTimer = new Timer(newCoolDown, false);
    }
    public override bool CanUseCard(Vector2Int gridPosition){
        return IGrid.Ins.IsValidGridPosition(gridPosition.x, gridPosition.y) && HaveEnoughResource();
    }
    public override SelectMessage CanSelectCard(){
        if(!HaveEnoughResource()) {
            return SelectMessage.InsuffientBanana;
        }
        else if(stack == 0) {
            return SelectMessage.Recovering;
        }
        return SelectMessage.CanSelect;
    }
    public override void UseCard(Vector2Int gridPos) {
        int startX = team == Team.Player ? -1 : GridSystem.Ins.width;
        IEntity e;
        BattleInfo.teamDict[team].resource -= cost;
        if(so.entitySO.IsContainTribes(new List<Tribe>() { Tribe.Tower })) {
            e = EContainer.Ins.CreateEntity(SingletonRegister.Get<PrefabRegisterSO>().builder, 0, gridPos.y, team);
        }
        else {
            e = EContainer.Ins.CreateEntity(so.entitySO, startX, gridPos.y, team);
        }
        cooldownTimer.Reset();
        stack--;
    }
    private bool HaveEnoughResource(){
        return BattleInfo.teamDict[team].resource >= cost;
    } 
    private class CostlyEffect : CardEffect, IOnDestroy{
        BattleCard effectedCard;
        int diff;
        public CostlyEffect(BattleCard effectedCard, int diff = 1, int duration = -1) : base(effectedCard, duration){
            this.diff = diff;
            effectedCard.cost += diff;
        }
        public void OnDestroy() {
            effectedCard.cost -= diff;
        }
    }
}