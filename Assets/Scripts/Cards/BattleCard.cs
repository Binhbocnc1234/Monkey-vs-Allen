using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

//Card không nên là một thực thể
public class BattleCard : IBattleCard
{
    public Team team;
    public int cardLevel;
    int maxStack = 1;
    int stack = 0;
    public void Initialize(CardSO so, Team team, int level) {
        if(so == null) {
            Debug.LogError("ApplyCardSO : so is null");
            return;
        }
        this.so = so;
        this.cost = so.cost;
        this.originalCost = so.cost;
        SetCoolDown(EnumConverter.Convert(so.cooldownType));
        cooldownTimer.SetCurTime(0);
        this.team = team;
        if(so is EnemyCardSO enemyCardSO) {
            this.maxStack = enemyCardSO.maxStack;
        }
        cardLevel = level;
    }
    public override void Update() {
        if(BattleInfo.gameState == GameState.Fighting) {
            if(cooldownTimer.Count() && stack < maxStack) {
                stack++;
                if (stack < maxStack) {
                    cooldownTimer.Reset();
                }
            }
        }
    }
    public override void SetCoolDown(float newCoolDown) {
        cooldownTimer = new Timer(newCoolDown, false);
    }
    public override bool CanUseCard(Vector2Int gridPosition){
        if (!IGrid.Ins.IsValidGridPosition(gridPosition.x, gridPosition.y)) return false;
        if (so.entitySO.IsContainTribes(new() { Tribe.Tower })) {
            if(gridPosition.x == 0 || gridPosition.x == IGrid.Ins.width - 1) {
                return false;
            }
            if(IGrid.Ins.GetCell(gridPosition).occupiedByTower) return false;
        }
        return CanSelectCard() == SelectMessage.CanSelect && cooldownTimer.isEnd &&
        HaveEnoughResource() && IGrid.Ins.openLanes[gridPosition.y];
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
        int startX = team == Team.Left ? -1 : IGrid.Ins.width;
        IEntity e;
        BattleInfo.teamDict[team].resource -= cost;
        if(so.entitySO.IsContainTribes(new List<Tribe>() { Tribe.Tower })) {
            IEntityRegistry.Ins.CreateBuilder(new EntitySetting {
                so = this.so.entitySO,
                lane = gridPos.y,
                x = gridPos.x,
                team = this.team,
            });
            
            // e = EContainer.Ins.CreateEntity(so.entitySO, gridPos.x, gridPos.y, team);
        }
        else {
            e = IEntityRegistry.Ins.CreateEntity(so.entitySO, startX, gridPos.y, team);
        }
        PlayerData.GetCardDataById(so.id).discovered = true;
        if (!BattleInfo.noCooldown) cooldownTimer.Reset();
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