using UnityEngine;


public partial class EnemyManager {
    [Header("Upgrade")]
    public int upgradeAfterUses = 3; // tương lai bỏ biến này
    public int upgradeWeight = 1;
    public int enemiesLeftToUpgrade = 4;
    public int upgradeLeft;
    private AlienResourceManager resourceM;
    // Accumulated card spending since last successful upgrade.
    [ReadOnly] public int resourceSpentSinceLastUpgrade = 0;
    bool firstUpgrade = true;
    int CalculateUpgradeLeft() {
        var cards = BattleInfo.teamDict[Team.Right].cards;
        
        float maxResourceConsumptionRate = 0f;
        for(int i = 0; i < cards.Count; ++i) {
            float cost = cards[i].GetSO().cost;
            float cooldown = EnumConverter.Convert(cards[i].GetSO().cooldownType);
            maxResourceConsumptionRate += cost / cooldown;
        }

        // Tốc độ sinh tài nguyên trên mỗi đơn vị upgrade
        float baseResourceGenerationRate = (float)BattleInfo.resourcePerGeneration / BattleInfo.resourceDelay;

        // Số lần nâng cấp tối đa cần thiết để bù đắp được tốc độ tiêu thụ nếu spam bài liên tục
        float upgradeTimes = maxResourceConsumptionRate / baseResourceGenerationRate;
        if(upgradeTimes >= 3) {
            return Mathf.FloorToInt(upgradeTimes);
        }
        else{
            return Mathf.CeilToInt(upgradeTimes);
        }
    }
    void ResetUpgradeScoreState() {
        resourceSpentSinceLastUpgrade = 0;
    }

    void RegisterCardSpendForUpgradeScore(int cardCost) {
        resourceSpentSinceLastUpgrade += Mathf.Max(0, cardCost);
    }

    internal bool CanChooseUpgradeActionAt(float lookahead) {
        if(upgradeLeft <= 0 || enemiesLeftToUpgrade != 0 || resourceM == null) {
            return false;
        }

        int expectedResource = EstimateEnemyResourceAfter(lookahead);
        return expectedResource >= resourceM.costToUpgrade;
    }

    internal float GetUpgradeActionScore(float lookahead) {
        if (firstUpgrade){
            return 10000f;
        }
        return resourceSpentSinceLastUpgrade * upgradeWeight;
    }

    void ExecuteUpgradeAction(float upgradeScore) {
        firstUpgrade = false;
        int resourceBefore = BattleInfo.teamDict[Team.Right].resource;
        resourceM.Upgrade();
        history.Push(new UpgradeTrace {
            upgradeNumber = resourceM.upgradeCnt,
            actionScore = upgradeScore,
            resourceBefore = resourceBefore,
            resourceAfter = BattleInfo.teamDict[Team.Right].resource,
        });
        enemiesLeftToUpgrade = upgradeAfterUses;
        upgradeLeft--;
        ResetUpgradeScoreState();
    }

    internal int EstimateEnemyResourceAfter(float lookahead) {
        int result = BattleInfo.teamDict[Team.Right].resource;
        if(resourceM == null || resourceM.upgradeCnt <= 0 || resourceM.resourceTimer == null) {
            return result;
        }

        float interval = resourceM.resourceTimer.totalTime;
        float firstGainAfter = resourceM.resourceTimer.remainingTime;
        if(lookahead < firstGainAfter) {
            return result;
        }

        int gainTick = 1 + Mathf.FloorToInt((lookahead - firstGainAfter) / interval);
        return result + gainTick * BattleInfo.resourcePerGeneration;
    }
}