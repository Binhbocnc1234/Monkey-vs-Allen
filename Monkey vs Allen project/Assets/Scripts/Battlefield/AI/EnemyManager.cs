using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public partial class EnemyManager : Singleton<EnemyManager> {
    // Candidate future checkpoints used by wait-vs-act decision.
    private static readonly float[] LOOKAHEAD_SECONDS = new float[] { 1f, 3f, 5f, 10f };
    // Reused list to avoid allocating a new List<Tribe> for every tower check.
    private static readonly List<Tribe> TOWER_TRIBE = new() { Tribe.Tower };
    private IGrid grid;
    
    [ReadOnly] public int meteorIndex = 0;
    private Timer waitingTimer, thinkingTimer;
    public float thinkingDelay = 1f;
    [Header("Information")]
    // Opponent team, the team is not controlled by AI
    public Team o_Team;
    public Team ourTeam;
    public List<float> urgencies = new();
    [Header("References")]
    public AITraceBuffer history;
    public void Initialize() {
        grid = IGrid.Ins;
        resourceM = AlienResourceManager.Ins;
        resourceM.Initialize();
        upgradeLeft = CalculateUpgradeLeft();
        enemiesLeftToUpgrade = 0;
        ResetUpgradeScoreState();
        waitingTimer = new Timer(BattleInfo.levelSO.alienWaitingTime, false);
        thinkingTimer = new Timer(thinkingDelay, reset: true);
        ourTeam = Team.Right;
        o_Team = Team.Left;

    }
    void Update() {
        if(BattleInfo.gameState != GameState.Fighting) { return; }
        if(waitingTimer.Count() == false) return;
        if(thinkingDelay > 0 && !thinkingTimer.Count()) return;
        // RefreshAssessmentDetails(0f);
        urgencies.Clear();
        urgencies.Add(GetUrgency(0));
        foreach(float lookAhead in LOOKAHEAD_SECONDS){
            urgencies.Add(GetUrgency(lookAhead));
        }
        BundleDecision nowBundle = FindBestUseCardBundle(0f, BattleInfo.teamDict[Team.Right].resource, true);
        UpgradeAction nowUpgrade = BuildUpgradeAction(0f);
        FutureBundleResult future = FindBestFutureUseCardBundle();

        AIAction bestImmediate = nowBundle;
        float bestImmediateAdjusted = GetAdjustedActionScore(nowBundle);
        if(nowUpgrade != null) {
            float nowUpgradeAdjusted = GetAdjustedActionScore(nowUpgrade);
            if(nowUpgradeAdjusted > bestImmediateAdjusted) {
                bestImmediate = nowUpgrade;
                bestImmediateAdjusted = nowUpgradeAdjusted;
            }
        }

        AIAction bestFutureAction = GetBestFutureAction(future, out float bestFutureAdjusted);

        if(bestFutureAction != null && bestFutureAdjusted > bestImmediateAdjusted) {
            history.Push(new WaitTrace {
                actionScore = bestFutureAdjusted,
                futureScore = $"{bestFutureAction.GetType().Name}: {bestFutureAdjusted:F1}",
                bestLookahead = bestFutureAction.lookahead,
            });
            return;
        }

        if(bestImmediate is UpgradeAction chosenUpgrade) {
            ExecuteUpgradeAction(chosenUpgrade.score);
            return;
        }

        int resourceBeforeBurst = BattleInfo.teamDict[Team.Right].resource;

        int usedCount = 0;
        int usedCost = 0;
        List<string> usedCardNames = new();
        for(int i = 0; i < nowBundle.usedCards.Count; ++i) {
            IBattleCard card = nowBundle.usedCards[i];
            Vector2Int spawnPos = new Vector2Int(grid.width - 1, nowBundle.lane);
            // Re-validate at execution time because board/resource state can change during this frame.
            if(card.CanUseCard(spawnPos) == false) { continue; }

            int resourceBeforeUse = BattleInfo.teamDict[Team.Right].resource;
            card.UseCard(spawnPos);
            int resourceAfterUse = BattleInfo.teamDict[Team.Right].resource;
            usedCount++;
            int cardCost = card.GetSO().cost;
            usedCost += cardCost;
            usedCardNames.Add(card.GetSO().name);
            RegisterCardSpendForUpgradeScore(cardCost);
        }

        if(usedCount > 0) {
            history.Push(new UseCardTrace {
                cardName = usedCardNames,
                lane = nowBundle.lane,
                cost = usedCost,
                actionScore = nowBundle.score,
                resourceBefore = resourceBeforeBurst,
                resourceAfter = BattleInfo.teamDict[Team.Right].resource,
            });

            enemiesLeftToUpgrade = Mathf.Max(0, enemiesLeftToUpgrade - usedCount);
        }
    }

    FutureBundleResult FindBestFutureUseCardBundle() {
        List<BundleDecision> bundleCandidates = new();
        List<UpgradeAction> upgradeCandidates = new();
        for(int i = 0; i < LOOKAHEAD_SECONDS.Length; ++i) {
            float lookahead = LOOKAHEAD_SECONDS[i];
            int futureResource = EstimateEnemyResourceAfter(lookahead);
            BundleDecision candidate = FindBestUseCardBundle(lookahead, futureResource, false);
            if(candidate.usedCards.Count > 0) {
                bundleCandidates.Add(candidate);
            }

            UpgradeAction upgradeCandidate = BuildUpgradeAction(lookahead);
            if(upgradeCandidate != null) {
                upgradeCandidates.Add(upgradeCandidate);
            }
        }

        bundleCandidates.Sort((a, b) => b.score.CompareTo(a.score));
        if(bundleCandidates.Count > FutureBundleResult.maxCount) {
            bundleCandidates.RemoveRange(FutureBundleResult.maxCount, bundleCandidates.Count - FutureBundleResult.maxCount);
        }

        return new FutureBundleResult {
            best = bundleCandidates,
            upgradeActions = upgradeCandidates,
        };
    }

    BundleDecision FindBestUseCardBundle(float lookahead, int resourceBudget, bool requireCurrentValidity) {
        var cards = BattleInfo.teamDict[Team.Right].cards;
        var openLanes = grid.GetOpenLanes();

        BundleDecision best = new BundleDecision { score = float.MinValue, lookahead = lookahead };

        for(int laneIndex = 0; laneIndex < openLanes.Count; ++laneIndex) {
            int lane = openLanes[laneIndex];
            GetAssessment(lane, lookahead);

            List<IBattleCard> laneCandidates = new();
            for(int i = 0; i < cards.Count; ++i) {
                IBattleCard card = cards[i];
                if(IsActionValidForTime(card, lane, lookahead, requireCurrentValidity) == false) { continue; }

                IEntity prefab = card.GetSO().entitySO.prefab.GetComponent<IEntity>();
                if(prefab == null) { continue; }
                laneCandidates.Add(card);
            }

            if(laneCandidates.Count == 0) { continue; }

            long maskLimit = 1L << laneCandidates.Count;
            for(long mask = 1; mask < maskLimit; ++mask) {
                float totalCost = 0;
                List<IBattleCard> picked = new();

                for(int bit = 0; bit < laneCandidates.Count; ++bit) {
                    if((mask & (1L << bit)) == 0) { continue; }
                    IBattleCard card = laneCandidates[bit];
                    totalCost += card.GetSO().cost;
                    if(totalCost > resourceBudget) {
                        totalCost = int.MaxValue;
                        break;
                    }
                    picked.Add(card);
                }

                if(totalCost == int.MaxValue) { continue; }
                float score = EvaluateBundle(picked, GetAssessment(lane, lookahead));
                if(score <= best.score) { continue; }

                best.usedCards.Clear();
                best.usedCards.AddRange(picked);
                best.cost = totalCost;
                best.score = score;
                best.lane = lane;
            }
        }

        if(best.score == float.MinValue) {
            best.score = 0f;
        }

        return best;
    }
    private float EvaluateBundle(List<IBattleCard> pickeds, LaneAssessment assessment) {
        float totalBundleDanger = 0, totalBundleDefend = 0;
        foreach(IBattleCard card in pickeds) {
            totalBundleDanger += card.GetSO().entitySO.prefab.GetComponent<IEntity>().GetAssessPoint(APType.Danger);
        }
        float distanceFactor = Mathf.Abs(EnumConverter.GetBasePosition(ourTeam) - assessment[ourTeam].defensiveFocusPoint) / IGrid.Ins.width;
        float ourPower = totalBundleDanger * totalBundleDefend * TeamSnapshot.GetUnitCountDebuff(pickeds.Count);
        ourPower += assessment[ourTeam].power * distanceFactor;
        float correlation = assessment[o_Team].power / ourPower;
        if(correlation < 1.05) {
            return ourPower / 2;
        }
        else {
            return ourPower * 2;
        }
    }
    UpgradeAction BuildUpgradeAction(float lookahead) {
        if(CanChooseUpgradeActionAt(lookahead) == false) {
            return null;
        }
        return new UpgradeAction {
            lookahead = lookahead,
            score = GetUpgradeActionScore(lookahead),
        };
    }

    float GetAdjustedActionScore(AIAction action) {
        return action.score - GetUrgency(action.lookahead);
    }

    AIAction GetBestFutureAction(FutureBundleResult future, out float bestAdjustedScore) {
        bestAdjustedScore = float.MinValue;
        AIAction bestAction = null;

        if(future.best != null) {
            for(int i = 0; i < future.best.Count; ++i) {
                BundleDecision candidate = future.best[i];
                float adjusted = GetAdjustedActionScore(candidate);
                if(adjusted > bestAdjustedScore) {
                    bestAdjustedScore = adjusted;
                    bestAction = candidate;
                }
            }
        }

        if(future.upgradeActions != null) {
            for(int i = 0; i < future.upgradeActions.Count; ++i) {
                UpgradeAction candidate = future.upgradeActions[i];
                float adjusted = GetAdjustedActionScore(candidate);
                if(adjusted > bestAdjustedScore) {
                    bestAdjustedScore = adjusted;
                    bestAction = candidate;
                }
            }
        }

        return bestAction;
    }

    bool IsActionValidForTime(IBattleCard card, int lane, float lookahead, bool requireCurrentValidity) {
        Vector2Int spawnPos = new Vector2Int(grid.width - 1, lane);
        if(requireCurrentValidity) {
            // Use game-native validation for "now" so stack/cost/cooldown rules stay consistent.
            return card.CanUseCard(spawnPos);
        }

        // Lightweight future validity approximation: grid/lane/cooldown/tower constraints only.
        if(IGrid.Ins.IsValidGridPosition(spawnPos.x, spawnPos.y) == false) { return false; }
        if(IGrid.Ins.openLanes[lane] == false) { return false; }

        if(card.cooldownTimer != null && card.cooldownTimer.isEnd == false && card.cooldownTimer.remainingTime > lookahead) {
            return false;
        }

        if(card.GetSO().entitySO.IsContainTribes(TOWER_TRIBE)) {
            if(spawnPos.x == 0 || spawnPos.x == IGrid.Ins.width - 1) {
                return false;
            }
            if(IGrid.Ins.GetCell(spawnPos).occupiedByTower) {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Tính toán thiệt hại do việc chờ quá lâu
    /// </summary>
    /// <param name="lookAhead"></param>
    /// <returns></returns>
    float GetUrgency(float lookAhead) {
        // Aggregate lane pressure and weighted protection demand into one anti-wait scalar.
        float urgency = 0f;
        for(int lane = 0; lane < grid.height; ++lane) {
            var assessment = GetAssessment(lane, lookAhead);
            urgency += assessment[o_Team].power * assessment[o_Team].totalNeedProtection / 100;
        }
        // Because during the waiting period, if AI hadn't used "Wait for future action," it could have done something to exert pressure.
        float timeWaste = lookAhead * BattleInfo.resourcePerGeneration * AlienResourceManager.Ins.upgradeCnt;
        return urgency + timeWaste;
    }

    // void RefreshAssessmentDetails(float lookAhead) {
    //     if(laneAssessmentDetails == null) {
    //         laneAssessmentDetails = new();
    //     }
    //     laneAssessmentDetails.Clear();
    //     for(int lane = 0; lane < grid.height; ++lane) {
    //         LaneAssessment detail = GetAssessment(lane, lookAhead);
    //         detail.lane = lane;
    //         laneAssessmentDetails.Add(detail);
    //     }
    // }

    class FutureBundleResult {
        public const int maxCount = 5;
        public List<BundleDecision> best; // top bundle theo score
        public List<UpgradeAction> upgradeActions; // theo từng lookAhead
    }
}