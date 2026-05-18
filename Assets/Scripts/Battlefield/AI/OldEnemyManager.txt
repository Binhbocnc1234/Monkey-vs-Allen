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
    public List<LaneAssessmentDetail> laneAssessmentDetails;
    public List<float> urgencies = new();
    [Header("References")]
    public AITraceBuffer history;
    public void Initialize() {
        grid = GridSystem.Ins;
        resourceM = AlienResourceManager.Ins;
        resourceM.Initialize();
        upgradeLeft = CalculateUpgradeLeft();
        enemiesLeftToUpgrade = 0;
        ResetUpgradeScoreState();
        waitingTimer = new Timer(BattleInfo.levelSO.alienWaitingTime, false);
        thinkingTimer = new Timer(thinkingDelay, reset: true);
    }
    void Update() {
        if(BattleInfo.gameState != GameState.Fighting) { return; }
        if(waitingTimer.Count() == false) return;
        if(thinkingDelay > 0 && !thinkingTimer.Count()) return;
        RefreshAssessmentDetails(0f);
        urgencies.Clear();
        urgencies.Add(GetUrgency(0));
        foreach(float lookAhead in LOOKAHEAD_SECONDS){
            urgencies.Add(GetUrgency(lookAhead));
        }
        BundleDecision nowBundle = FindBestUseCardBundle(0f, BattleInfo.teamDict[Team.Enemy].resource, true);
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

        int resourceBeforeBurst = BattleInfo.teamDict[Team.Enemy].resource;

        int usedCount = 0;
        int usedCost = 0;
        List<string> usedCardNames = new();
        for(int i = 0; i < nowBundle.actions.Count; ++i) {
            IBattleCard card = nowBundle.actions[i];
            Vector2Int spawnPos = new Vector2Int(grid.width - 1, nowBundle.lane);
            // Re-validate at execution time because board/resource state can change during this frame.
            if(card.CanUseCard(spawnPos) == false) { continue; }

            int resourceBeforeUse = BattleInfo.teamDict[Team.Enemy].resource;
            card.UseCard(spawnPos);
            int resourceAfterUse = BattleInfo.teamDict[Team.Enemy].resource;
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
                resourceAfter = BattleInfo.teamDict[Team.Enemy].resource,
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
            if(candidate.actions.Count > 0) {
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
        var cards = BattleInfo.teamDict[Team.Enemy].cards;
        var openLanes = GridSystem.Ins.GetOpenLanes();

        BundleDecision best = new BundleDecision { score = float.MinValue, lookahead = lookahead };

        for(int laneIndex = 0; laneIndex < openLanes.Count; ++laneIndex) {
            int lane = openLanes[laneIndex];
            LaneAssessmentDetail laneAssessment = GetCurrentPlayerPressure(lane, lookahead);

            List<IBattleCard> laneCandidates = new();
            List<float> laneCandidateDanger = new();
            List<float> laneCandidateDefend = new();
            for(int i = 0; i < cards.Count; ++i) {
                IBattleCard card = cards[i];
                if(IsActionValidForTime(card, lane, lookahead, requireCurrentValidity) == false) { continue; }

                IEntity prefab = card.GetSO().entitySO.prefab.GetComponent<IEntity>();
                if(prefab == null) { continue; }
                laneCandidates.Add(card);
                laneCandidateDanger.Add(Mathf.Max(0f, prefab.GetAssessPoint(APType.Danger)));
                laneCandidateDefend.Add(Mathf.Max(0f, prefab.GetAssessPoint(APType.Defend)));
            }

            if(laneCandidates.Count == 0) { continue; }

            long maskLimit = 1L << laneCandidates.Count;
            for(long mask = 1; mask < maskLimit; ++mask) {
                float totalCost = 0;
                float cardDangerSum = 0f;
                float cardDefendSum = 0f;
                List<IBattleCard> picked = new();

                for(int bit = 0; bit < laneCandidates.Count; ++bit) {
                    if((mask & (1L << bit)) == 0) { continue; }
                    IBattleCard card = laneCandidates[bit];
                    totalCost += card.GetSO().cost;
                    if(totalCost > resourceBudget) {
                        totalCost = int.MaxValue;
                        break;
                    }

                    cardDangerSum += laneCandidateDanger[bit];
                    cardDefendSum += laneCandidateDefend[bit];
                    picked.Add(card);
                }

                if(totalCost == int.MaxValue) { continue; }

                float totalDanger = laneAssessment.danger + cardDangerSum;
                float totalDefend = laneAssessment.survivability + cardDefendSum;
                float score = totalDanger * totalDefend;
                if(score <= best.score) { continue; }

                best.actions.Clear();
                best.actions.AddRange(picked);
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

    int EstimateEnemyResourceAfter(float lookahead) {
        int result = BattleInfo.teamDict[Team.Enemy].resource;
        if(resourceM == null || resourceM.upgradeCnt <= 0 || resourceM.resourceTimer == null) {
            return result;
        }

        // Resource ticks are periodic after the first remaining-time boundary.
        float interval = resourceM.resourceTimer.totalTime;
        float firstGainAfter = resourceM.resourceTimer.remainingTime;
        if(lookahead < firstGainAfter) {
            return result;
        }

        int gainTick = 1 + Mathf.FloorToInt((lookahead - firstGainAfter) / interval);
        return result + gainTick * BattleInfo.resourcePerGeneration;
    }


    float GetUrgency(float lookAhead) {
        // Aggregate lane pressure and weighted protection demand into one anti-wait scalar.
        float urgency = 0f;
        for(int lane = 0; lane < grid.height; ++lane) {
            LaneAssessmentDetail laneAssessment = GetCurrentPlayerPressure(lane, lookAhead);
            float lanePower = laneAssessment.power;
            if(lanePower <= 0f) { continue; }
            urgency += lanePower * laneAssessment.totalNeedProtection / 100;
        }
        // Because during the waiting period, if AI hadn't used "Wait for future action," it could have done something to exert pressure.
        float timeWaste = lookAhead * BattleInfo.resourcePerGeneration * AlienResourceManager.Ins.upgradeCnt;
        return urgency + timeWaste;
    }

    void RefreshAssessmentDetails(float lookAhead) {
        if(laneAssessmentDetails == null) {
            laneAssessmentDetails = new();
        }
        laneAssessmentDetails.Clear();
        for(int lane = 0; lane < grid.height; ++lane) {
            LaneAssessmentDetail detail = GetCurrentPlayerPressure(lane, lookAhead);
            detail.lane = lane;
            laneAssessmentDetails.Add(detail);
        }
    }
    
    LaneAssessmentDetail GetCurrentPlayerPressure(int lane, float lookAhead = 0) {
        if(lookAhead < 0) {
            Debug.LogError("[EnemyManager::GetCurrentPlayerPressure] Invalid parameter lookAhead");
            lookAhead = 0;
        }
        var entities = EContainer.Ins.GetEntitiesByLane(lane);
        LaneAssessmentDetail ans = new();

        IEntity playerLeader = FindLaneLeader(entities, Team.Player);
        IEntity enemyLeader = FindLaneLeader(entities, Team.Enemy);
        if(playerLeader == null) {
            return ans;
        }

        ans.leader = playerLeader;
        ans.enemyLeader = enemyLeader;

        TeamLaneSnapshot playerNow = ComputeTeamSnapshot(entities, Team.Player, playerLeader, lookAhead);
        TeamLaneSnapshot enemyNow = ComputeTeamSnapshot(entities, Team.Enemy, enemyLeader, lookAhead);

        ans.opponentCount = playerNow.unitCount;

        float timeToFirstContact = ForecastTimeToFirstContact(entities);
        bool willFightBeforeLookAhead = enemyLeader != null && timeToFirstContact <= lookAhead;

        float playerDanger = playerNow.danger;
        float playerSurvivability = playerNow.survivability;
        float enemyDanger = enemyNow.danger;
        float enemySurvivability = enemyNow.survivability;

        if(willFightBeforeLookAhead) {
            float combatDuration = Mathf.Max(0f, lookAhead - timeToFirstContact);
            float projectedDamageToEnemy = playerNow.danger * combatDuration;
            float projectedDamageToPlayer = enemyNow.danger * combatDuration;

            enemySurvivability = Mathf.Max(0f, enemyNow.survivability - projectedDamageToEnemy);
            playerSurvivability = Mathf.Max(0f, playerNow.survivability - projectedDamageToPlayer);

            float playerAliveRatio = playerNow.survivability > 0f ? playerSurvivability / playerNow.survivability : 0f;
            float enemyAliveRatio = enemyNow.survivability > 0f ? enemySurvivability / enemyNow.survivability : 0f;
            playerDanger = playerNow.danger * playerAliveRatio;
            enemyDanger = enemyNow.danger * enemyAliveRatio;
        }

        ans.playerDanger = playerDanger;
        ans.playerSurvivability = playerSurvivability;
        ans.playerPower = playerDanger * playerSurvivability * Mathf.Max(0.5f, 1.05f - ans.opponentCount * 0.05f);

        ans.enemyDanger = enemyDanger;
        ans.enemySurvivability = enemySurvivability;
        ans.enemyPower = enemyDanger * enemySurvivability;

        float enemyNeedProtectionBase = 0f;
        for(int i = 0; i < entities.Length; ++i) {
            IEntity e = entities[i];
            if(e.team != Team.Enemy) { continue; }
            enemyNeedProtectionBase += e.GetAssessPoint(APType.NeedProtection);
        }
        float projectedEnemyHpRatio = enemyNow.survivability > 0f ? enemySurvivability / enemyNow.survivability : 0f;
        float hpWeight = 1f + (1f - projectedEnemyHpRatio) * 4f;
        ans.totalNeedProtection = enemyNeedProtectionBase * hpWeight;

        ans.SyncLegacyFieldsFromPlayerMetrics();
        return ans;
    }

    IEntity FindLaneLeader(IEntity[] entities, Team team) {
        float farthest = float.MinValue;
        IEntity leader = null;
        for(int i = 0; i < entities.Length; ++i) {
            IEntity e = entities[i];
            if(e.team != team) { continue; }

            float distanceScore = team == Team.Player ? e.DistanceToBase() : e.DistanceToOpponentBase();
            if(distanceScore > farthest) {
                farthest = distanceScore;
                leader = e;
            }
        }
        return leader;
    }

    TeamLaneSnapshot ComputeTeamSnapshot(IEntity[] entities, Team team, IEntity leader, float lookAhead) {
        TeamLaneSnapshot snapshot = new TeamLaneSnapshot();
        if(leader == null) {
            return snapshot;
        }

        for(int i = 0; i < entities.Length; ++i) {
            IEntity e = entities[i];
            if(e.team != team) { continue; }

            snapshot.unitCount++;
            float distanceToLeader = Mathf.Max(0f, e.DistanceTo(leader) - e[ST.Range] - e[ST.MoveSpeed] * lookAhead);
            float speedSafe = Mathf.Max(e[ST.MoveSpeed], 0.1f);
            float distanceFactor = 1f - distanceToLeader / speedSafe / 4f;
            if(distanceFactor < 0f) { continue; }

            snapshot.danger += e.GetAssessPoint(APType.Danger) * distanceFactor;
            float defend = e.GetAssessPoint(APType.Defend);
            if(e.so.IsContainTribes(new() { Tribe.Target })) {
                snapshot.survivability += defend / 4f;
            }
            else {
                snapshot.survivability += defend * distanceFactor;
            }
        }
        return snapshot;
    }

    float ForecastTimeToFirstContact(IEntity[] entities) {
        float best = float.PositiveInfinity;
        for(int i = 0; i < entities.Length; ++i) {
            IEntity a = entities[i];
            if(a.team != Team.Player) { continue; }

            for(int j = 0; j < entities.Length; ++j) {
                IEntity b = entities[j];
                if(b.team != Team.Enemy) { continue; }

                float distance = a.DistanceTo(b);
                float reach = Mathf.Max(a[ST.Range], b[ST.Range]);
                float closeDistance = Mathf.Max(0f, distance - reach);
                float relativeSpeed = Mathf.Max(0.1f, a[ST.MoveSpeed] + b[ST.MoveSpeed]);
                float time = closeDistance / relativeSpeed;
                if(time < best) {
                    best = time;
                }
            }
        }
        return best;
    }

    float PredictFutureHealthPercentage(IEntity protectedEntity, IEntity[] opponents, float lookAhead) {
        float currentHpPercent = Mathf.Clamp01(protectedEntity.GetHealthPercentage());
        if(lookAhead <= 0f) {
            return currentHpPercent;
        }

        float incomingDangerPressure = 0f;
        for(int i = 0; i < opponents.Length; ++i) {
            IEntity opponent = opponents[i];
            if(opponent.team != Team.Player) { continue; }

            float approachTime = EstimateApproachTime(opponent, protectedEntity);
            if(approachTime >= lookAhead) { continue; }

            float contactDuration = lookAhead - approachTime;
            incomingDangerPressure += opponent.GetAssessPoint(APType.Danger) * contactDuration;
        }

        // Normalize projected damage by target defend score to get a future HP percentage approximation.
        float defendScale = Mathf.Max(1f, protectedEntity.GetAssessPoint(APType.Defend));
        float estimatedHpLoss = incomingDangerPressure / defendScale;
        return Mathf.Clamp01(currentHpPercent - estimatedHpLoss);
    }

    float EstimateApproachTime(IEntity attacker, IEntity target) {
        float closeDistance = Mathf.Max(attacker.DistanceTo(target) - attacker[ST.Range], 0f);
        float moveSpeed = Mathf.Max(attacker[ST.MoveSpeed], 0.1f);
        return closeDistance / moveSpeed;
    }

    struct TeamLaneSnapshot {
        public float danger;
        public float survivability;
        public int unitCount;
    }

    class FutureBundleResult {
        public const int maxCount = 5;
        public List<BundleDecision> best; // top bundle theo score
        public List<UpgradeAction> upgradeActions; // theo từng lookAhead
    }
}