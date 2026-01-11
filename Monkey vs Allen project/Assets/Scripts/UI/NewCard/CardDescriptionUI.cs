using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDescriptionUI : Singleton<CardDescriptionUI> {
    public CardSO so;
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private TMP_Text cardName, tribeTMP;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image model;
    [SerializeField] private StatUI statUIPrefab;
    [SerializeField] private TraitUI traitUIPrefab;
    [SerializeField] private Transform traitText;
    [SerializeField] private CardIconMapSO iconMap;
    [SerializeField] public HideAndShowUIManager hideAndShowManager;
    [SerializeField] private StatUIPool statUIPool;
    [SerializeField] private TraitUIPool traitUIPool;
    [SerializeField] private Image shardProgress;
    [SerializeField] private TMP_Text shardCount;
    protected override void Awake() {
        base.Awake();
        hideAndShowManager.HideAllImmediately();
        panel.alpha = 0;
    }
    public void Initialize(CardSO so) {
        Show();
        if (this.so == so) {
            return;
        }
        this.so = so;
        cardName.text = so.cardName;
        cardName.color = EnumConverter.FromRarityToColor(so.cardRarity);
        model.sprite = so.sprite;
        string tribeText = "- ";
        // Tribe
        foreach(Tribe tribe in so.entitySO.tribes) {
            tribeText += tribe.ToString() + ' ';
        }
        tribeText += " -";
        tribeTMP.text = tribeText;

        // Clear using pool-aware release
        statUIPool.Clear();
        StatUI healthUI = GetStatUI();
        healthUI.Initialize(iconMap.healthIcon, so.entitySO.health);

        if(so.entitySO.canAttack) {
            StatUI damageUI = GetStatUI();
            StatUI attackRangeUI = GetStatUI();
            StatUI attackSpeedUI = GetStatUI();
            damageUI.Initialize(iconMap.damageIcon, so.entitySO.damage);
            attackRangeUI.Initialize(iconMap.attackRangeIcon, so.entitySO.attackRange);
            attackSpeedUI.Initialize(iconMap.attackSpeedIcon, so.entitySO.attackSpeed);
        }
        if(!so.entitySO.tribes.Contains(Tribe.Tower)) {
            StatUI moveSpeedUI = GetStatUI();
            moveSpeedUI.Initialize(iconMap.moveSpeedIcon, so.entitySO.moveSpeed);
        }
        if(so.entitySO.traits.Count > 0) {
            traitText.gameObject.SetActive(true);
            traitUIPool.Clear();
            foreach(TraitType traitType in so.entitySO.traits) {
                TraitUI traitUI = GetTraitUI();
                traitUI.Initialize(iconMap.GetIconByEnum(traitType), traitType.ToString());
            }
        }
        else {
            traitText.gameObject.SetActive(false);
        }
        description.text = "Description: " + so.description;
        if(Application.isPlaying) {
            CardData cardData = PlayerData.GetCardDataById(so.id);
            shardCount.text = $"{cardData.shards}/{CardSO.UPGRADE_THRESHOLD[cardData.level]}";
            shardProgress.fillAmount = cardData.shards / (float)CardSO.UPGRADE_THRESHOLD[cardData.level];
        }
        Debug.Log("[CardDescriptionUI] Finish Initialization");
    }
    public void Close() {
        panel.alpha = 1;
        LeanTween.cancel(panel.gameObject);
        panel.LeanAlpha(0, 0.25f);
        hideAndShowManager.HideAll();
    }
    private void Show() {
        panel.alpha = 0;
        LeanTween.cancel(panel.gameObject);
        panel.LeanAlpha(1, 0.25f);
        hideAndShowManager.ShowAll();
    }
    // Pool-aware helpers
    private StatUI GetStatUI() {
        StatUI ui = statUIPool.Get();
        ui.gameObject.SetActive(true);
        return ui;
    }

    private TraitUI GetTraitUI() {
        TraitUI ui = traitUIPool.Get();
        ui.gameObject.SetActive(true);
        return ui;
    }

}
