using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class BattleCardUI : CardUI {
    public Image cooldownMask;
    [SerializeField] private IBattleCard battleCard;
    protected RectTransform cooldownMaskRect;
    protected float initialCooldownHeight;
    protected override void Start() {
        base.Start();
        cooldownMask.gameObject.SetActive(false);
        cooldownMaskRect = cooldownMask.rectTransform;
        initialCooldownHeight = cooldownMaskRect.rect.height;
        BattleInfo.OnStateChanged += () => {
            cooldownMask.gameObject.SetActive(BattleInfo.gameState == GameState.Fighting);
        };
        OnClickEvent += () => {
            if (BattleInfo.gameState == GameState.Fighting && battleCard != null)   GetCardAvailability();
        };
    }
    void GetCardAvailability() {
        SelectMessage selectMessage = battleCard.CanSelectCard();
        if(selectMessage == SelectMessage.InsuffientBanana) {
            CostInsuffientAnimation.Instantiate(this);
        }
        else if(selectMessage == SelectMessage.CanSelect) {
            PointerUI.Ins.Initialize(battleCard);
        }
    }
    void Update() {
        if (BattleInfo.gameState == GameState.Fighting && battleCard != null)
        {
            UpdateBattleCard();
        }
    }
    void UpdateBattleCard() {
        float percent = battleCard.cooldownTimer.GetPercent();
        RectTransformExtensions.SetTop(cooldownMaskRect, initialCooldownHeight * percent);
        if(battleCard.originalCost == battleCard.cost) {
            appearance.cost.color = Color.white;
        }
        else if(battleCard.cost > battleCard.originalCost) {
            appearance.cost.color = Color.red;
        }
        else {
            appearance.cost.color = Color.green;
        }
    }
    public void SetBattleCard(IBattleCard battleCard) {
        this.battleCard = battleCard;
        ApplyCardSO(battleCard.GetSO());
    }
}