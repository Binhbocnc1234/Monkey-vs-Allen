using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class BattleCardUI : CardUI {
    public Image cooldownMask;
    public IBattleCard card;
    protected RectTransform cooldownMaskRect;
    protected float initialCooldownHeight;
    [SerializeField] private Button button;
    protected override void Start() {
        base.Start();
        cooldownMask.gameObject.SetActive(false);
        cooldownMaskRect = cooldownMask.rectTransform;
        initialCooldownHeight = cooldownMaskRect.rect.height;
        button.interactable = false;
        BattleInfo.OnStateChanged += () => {
            if(BattleInfo.gameState == GameState.Fighting) {
                button.interactable = true;
                cooldownMask.gameObject.SetActive(true);
            }
        };
        OnClickEvent += GetCardAvailability;
    }
    void GetCardAvailability() {
        SelectMessage selectMessage = card.CanSelectCard();
        if(selectMessage == SelectMessage.InsuffientBanana) {
            CostInsuffientAnimation.Instantiate(this);
        }
        else if(selectMessage == SelectMessage.CanSelect) {
            PointerUI.Ins.Initialize(card);
        }
    }
    void Update() {
        if (card != null)
        {
            UpdateBattleCard();
        }
    }
    void UpdateBattleCard() {
        RectTransformExtensions.SetTop(cooldownMaskRect, initialCooldownHeight * card.cooldownTimer.GetPercent());
        if(card.originalCost == card.cost) {
            appearance.cost.color = Color.white;
        }
        else if(card.cost > card.originalCost) {
            appearance.cost.color = Color.red;
        }
        else {
            appearance.cost.color = Color.green;
        }
    }
}