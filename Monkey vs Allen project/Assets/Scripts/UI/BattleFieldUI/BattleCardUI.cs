using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class BattleCardUI : CardUI {
    public Image cooldownMask;
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
    }
    protected void Update() {

    }
    public void UpdateBattleCard(IBattleCard card) {
        RectTransformExtensions.SetTop(cooldownMaskRect, initialCooldownHeight * card.GetCooldownPercent());
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