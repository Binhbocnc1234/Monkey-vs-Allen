using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

//Card không nên là một thực thể
public class BattleCard : IBattleCard
{
    public BattleCardUI cardUI;
    public Team team;
    public BattleCard(CardSO so, Team team, BattleCardUI cardUI) : this(so, team){
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
    public BattleCard(CardSO so, Team team) {
        if(so == null) {
            Debug.LogError("ApplyCardSO : so is null");
            return;
        }
        this.so = so;
        this.cost = so.cost;
        this.originalCost = so.cost;
        SetCoolDown(EnumConverter.Convert(so.cooldownType));
        this.team = team;
    }
    public override void Update() {
        if(isActive) {
            cooldownTimer.Count();
            if (cardUI != null) {
                cardUI.UpdateBattleCard(this);
            }
        }
    }
    // public override CardEffect ReceiveEffect(CardEffectType effectType){
    //     CardEffect effect = new CostlyEffect(this);
    //     switch(effectType){
    //         case CardEffectType.Costly: effect = new CostlyEffect(this); break;
    //         default:
    //             Debug.LogError("MonkeyCard::ReceiveEffect: CardEffectType not found");
    //             break;
    //     }
    //     // EffectManager.Ins.AddElement(effect);
    //     return effect;
    // }
    public override bool CanUseCard(Vector2Int gridPosition){
        if (IGrid.Ins.IsValidGridPosition(gridPosition.x, gridPosition.y) == false){
            return false;
        }
        ICell cell = IGrid.Ins.GetCell(gridPosition);
        if (cell.hasBlock == false){
            return false;
        }

        return cell.hasBlock && !cell.hasTower && HaveEnoughBanana();
    }
    public override SelectMessage CanSelectCard(){
        if(!HaveEnoughBanana()) {
            return SelectMessage.InsuffientBanana;
        }
        else if(!cooldownTimer.isEnd) {
            return SelectMessage.Recovering;
        }
        return SelectMessage.CanSelect;
    }
    public override void UseCard(Vector2Int gridPos) {
        int startX = team == Team.Player ? 0 : GridSystem.Ins.width - 1;
        if(so.entitySO.IsContainTribes(new List<Tribe>(){Tribe.Tower})) {
            if(so is MonkeyCardSO) {
                BattleInfo.ChangeBananaCnt(-cost);
            }
            else {
                BattleInfo.ChangeAllenanaCnt(-cost);
            }
            EContainer.Ins.CreateEntity(SingletonRegister.Get<PrefabRegisterSO>().builder, new Vector2Int(0, gridPos.y), team);
        }
        else {
            if(so is MonkeyCardSO) {
                BattleInfo.ChangeBananaCnt(-cost);
            }
            else {
                BattleInfo.ChangeAllenanaCnt(-cost);
            }
            EContainer.Ins.CreateEntity(so.entitySO, new Vector2Int(startX, gridPos.y), team);
        }
        cooldownTimer.Reset();
    }
    private bool HaveEnoughBanana(){
        if (so is MonkeyCardSO) {
            return BattleInfo.BananaCnt >= cost;
        }
        else {
            return BattleInfo.AllenanaCnt >= cost;
        }
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