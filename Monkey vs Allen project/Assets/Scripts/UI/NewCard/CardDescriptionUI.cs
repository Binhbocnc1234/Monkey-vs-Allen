using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class CardDescriptionUI : Singleton<CardDescriptionUI> {
    public CardSO so;
    [SerializeField] private CanvasGroup panel;
    [SerializeField] private TMP_Text cardName, tribeTMP;
    [SerializeField] private TMP_Text description;
    [SerializeField] private Image model;
    [SerializeField] private StatUI statUIPrefab;
    [SerializeField] private TraitUI traitUIPrefab;
    [SerializeField] private SkillUI skillUIPrefab;
    [SerializeField] private Transform traitText;
    [SerializeField] public HideAndShowUIManager hideAndShowManager;
    [SerializeField] private StatUIPool statUIPool;
    [SerializeField] private TraitUIPool traitUIPool;
    [SerializeField] private Transform skillContainer;
    [SerializeField] private Image shardProgress;
    [SerializeField] private TMP_Text shardCount;
    [SerializeField] private ScrollResetter scrollResetter;
    [SerializeField] private TMP_Text upgradePrefab;
    [SerializeField] private Transform upgradeContainer;
    protected override void Awake() {
        base.Awake();
        hideAndShowManager.HideAllImmediately();
        panel.alpha = 0;
    }
    public void Initialize(CardSO so) {
        Initialize(so, PlayerData.GetCardDataById(so.id).level);
    }
    public void Initialize(CardSO so, int level) {
        Show();
        if(this.so == so) {
            return;
        }
        EntitySO enSO = so.entitySO;
        UDictionary<ST, float> stats = enSO.GetEntityStats(level);
        this.so = so;
        cardName.text = so.cardName;
        cardName.color = EnumConverter.FromRarityToColor(so.cardRarity);
        model.sprite = so.sprite;
        string tribeText = "- ";
        // Tribe
        foreach(Tribe tribe in enSO.tribes) {
            tribeText += tribe.ToString() + ' ';
        }
        tribeText += " -";
        tribeTMP.text = tribeText;

        // Stat container
        statUIPool.Clear();
        foreach(var stat in enSO.GetEntityStats(level)) {
            StatUI statUI = GetStatUI();
            statUI.Initialize(stat.Key, stat.Value);
        }
        // Trait container
        if(enSO.traits.Count > 0) {
            traitText.gameObject.SetActive(true);
            traitUIPool.Clear();
            foreach(EffectType traitType in enSO.traits) {
                TraitUI traitUI = GetTraitUI();
                traitUI.Initialize(traitType);
            }
        }
        else {
            traitText.gameObject.SetActive(false);
        }
        // Skills
        skillContainer.DestroyAllChildren();
        List<Upgrade> upgrades = enSO.GetUpgrades();
        List<SkillSO> skillSOs = new List<SkillSO>(enSO.unlockedSkillInFirstLevel);
        SkillSO lockedSkill = null;
        for(int i = 0; i < level - 1; ++i) {
            Upgrade up = upgrades[i];
            if(up is UnlockSkill unlockSkill) {
                lockedSkill = unlockSkill.skillSO;
            }
        }
        foreach(SkillSO skillSO in skillSOs) {
            int upgradeCount = 0;
            for(int i = 0; i < level - 1; ++i) {
                Upgrade upgrade = upgrades[i];
                if(upgrade is SkillUpgrade skillUp && skillUp.skillSO == skillSO) {
                    upgradeCount++;
                }
            }
            Instantiate(skillUIPrefab, skillContainer).UpdateInfo(skillSO, true, upgradeCount);
        }
        if (lockedSkill != null) {
            Instantiate(skillUIPrefab, skillContainer).UpdateInfo(lockedSkill, false, 0);
        }
        // Upgrades
        upgradeContainer.DestroyAllChildren();
        CreateStatUpgradeText(enSO.level_2.stat, enSO.level_2.amount, level >= 2);
        CreateUpgradeText("Unlock skill: " + enSO.unlockedSkillInLevel3.skillName, level >= 3);
        CreateStatUpgradeText(enSO.level_4.stat, enSO.level_4.amount, level >= 4);
        CreateUpgradeText($"Better {enSO.upgradedSkillAtLv5.skillName}", level >= 5);

        description.text = "Description: " + so.description.GetString();
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
        scrollResetter.OpenScrollRect();
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

    private void CreateStatUpgradeText(ST stat, float value, bool unlocked) {
        TMP_Text text = Instantiate(upgradePrefab, upgradeContainer);
        text.text = $"{stat} increases by {value}";
        if(!unlocked) {
            text.color = Color.gray;
        }
    }
    private void CreateUpgradeText(string content, bool unlocked) {
        TMP_Text text = Instantiate(upgradePrefab, upgradeContainer);
        text.text = content;
        if(!unlocked) {
            text.color = Color.gray;
        }
    }
}
