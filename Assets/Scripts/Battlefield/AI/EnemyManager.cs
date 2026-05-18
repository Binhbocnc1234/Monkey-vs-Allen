using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public partial class EnemyManager : Singleton<EnemyManager> {
    // Reused list to avoid allocating a new List<Tribe> for every tower check.
    private static readonly List<Tribe> TOWER_TRIBE = new() { Tribe.Tower };
    private const float MaxDecisionLookaheadSeconds = 30f;
    private IGrid grid;
    
    [ReadOnly] public int meteorIndex = 0;
    private Timer waitingTimer, thinkingTimer;
    public float thinkingDelay = 1f;
    [Header("Information")]
    // Opponent team, the team is not controlled by AI
    public Team o_Team;
    public Team ourTeam;
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

        AIAction bestAction = FindBestAction();
        if(bestAction == null) {
            return;
        }

        if(bestAction.lookahead > 0f) {
            history.Push(new WaitTrace {
                actionScore = bestAction.score,
                futureScore = $"{bestAction.GetType().Name}: {bestAction.score:F1}",
                bestLookahead = bestAction.lookahead,
            });
            return;
        }

        if(bestAction is UpgradeAction chosenUpgrade) {
            ExecuteUpgradeAction(chosenUpgrade.score);
            return;
        }

        if(bestAction is BundleDecision chosenBundle) {
            ExecuteBundle(chosenBundle);
        }
    }

    AIAction FindBestAction() {
        AIAction bestAction = null;
        float bestScore = float.MinValue;

        BundleDecision bestBundle = FindBestSimulatedBundle();
        if(bestBundle != null) {
            bestAction = bestBundle;
            bestScore = bestBundle.score;
        }

        UpgradeAction bestUpgrade = BuildBestUpgradeAction();
        if(bestUpgrade != null && bestUpgrade.score > bestScore) {
            bestAction = bestUpgrade;
            bestScore = bestUpgrade.score;
        }

        return bestAction;
    }

    BundleDecision FindBestSimulatedBundle() {
        var cards = BattleInfo.teamDict[ourTeam].cards;
        var openLanes = grid.GetOpenLanes();

        BundleDecision best = null;
        float bestScore = float.MinValue;

        for(int laneIndex = 0; laneIndex < openLanes.Count; ++laneIndex) {
            int lane = openLanes[laneIndex];
            IEntity[] laneEntities = IEntityRegistry.Ins.GetEntitiesByLane(lane);

            List<IBattleCard> laneCandidates = new();
            for(int i = 0; i < cards.Count; ++i) {
                IBattleCard card = cards[i];
                if(CanConsiderCardOnLane(card, lane) == false) {
                    continue;
                }

                if(card.GetSO().entitySO.prefab == null) {
                    continue;
                }

                laneCandidates.Add(card);
            }

            if(laneCandidates.Count == 0) {
                continue;
            }

            long maskLimit = 1L << laneCandidates.Count;
            for(long mask = 1; mask < maskLimit; ++mask) {
                List<IBattleCard> picked = new();
                int totalCost = 0;
                float cooldownWait = 0f;

                for(int bit = 0; bit < laneCandidates.Count; ++bit) {
                    if((mask & (1L << bit)) == 0) {
                        continue;
                    }

                    IBattleCard card = laneCandidates[bit];
                    totalCost += card.GetSO().cost;
                    cooldownWait = Mathf.Max(cooldownWait, GetCardCooldownWait(card));
                    picked.Add(card);
                }

                float resourceWait = CalculateLookaheadToAfford(totalCost);
                if(float.IsPositiveInfinity(resourceWait)) {
                    continue;
                }

                float lookahead = Mathf.Max(resourceWait, cooldownWait);
                if(lookahead > MaxDecisionLookaheadSeconds) {
                    continue;
                }

                float score = Simulator.EvaluateBundle(laneEntities, picked, ourTeam, o_Team, grid.width, lookahead);
                if(score <= bestScore) {
                    continue;
                }

                bestScore = score;
                best = new BundleDecision {
                    lane = lane,
                    lookahead = lookahead,
                    score = score,
                    cost = totalCost,
                    usedCards = new List<IBattleCard>(picked),
                };
            }
        }

        return best;
    }

    UpgradeAction BuildBestUpgradeAction() {
        if(resourceM == null) {
            return null;
        }

        float lookahead = CalculateLookaheadToAfford(resourceM.costToUpgrade);
        if(float.IsPositiveInfinity(lookahead)) {
            return null;
        }

        return BuildUpgradeAction(lookahead);
    }

    float CalculateLookaheadToAfford(int cost) {
        int currentResource = BattleInfo.teamDict[ourTeam].resource;
        if(cost <= currentResource) {
            return 0f;
        }

        if(resourceM == null || resourceM.resourceTimer == null || resourceM.upgradeCnt <= 0) {
            return float.PositiveInfinity;
        }

        int missingResource = cost - currentResource;
        int gainPerTick = BattleInfo.resourcePerGeneration;
        if(gainPerTick <= 0) {
            return float.PositiveInfinity;
        }

        int ticksNeeded = Mathf.CeilToInt(missingResource / (float)gainPerTick);
        float interval = Mathf.Max(0.01f, resourceM.resourceTimer.totalTime);
        float firstGainAfter = Mathf.Max(0f, resourceM.resourceTimer.remainingTime);
        return firstGainAfter + Mathf.Max(0, ticksNeeded - 1) * interval;
    }

    float GetCardCooldownWait(IBattleCard card) {
        if(card.cooldownTimer == null || card.cooldownTimer.isEnd) {
            return 0f;
        }

        return Mathf.Max(0f, card.cooldownTimer.remainingTime);
    }

    bool CanConsiderCardOnLane(IBattleCard card, int lane) {
        Vector2Int spawnPos = GetSpawnPosition(lane);
        if(IGrid.Ins.IsValidGridPosition(spawnPos.x, spawnPos.y) == false) { return false; }
        if(IGrid.Ins.openLanes[lane] == false) { return false; }

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

    Vector2Int GetSpawnPosition(int lane) {
        return ourTeam == Team.Left ? new Vector2Int(0, lane) : new Vector2Int(grid.width - 1, lane);
    }

    void ExecuteBundle(BundleDecision bundle) {
        int resourceBeforeBurst = BattleInfo.teamDict[ourTeam].resource;

        int usedCount = 0;
        int usedCost = 0;
        List<string> usedCardNames = new();
        Vector2Int spawnPos = GetSpawnPosition(bundle.lane);

        for(int i = 0; i < bundle.usedCards.Count; ++i) {
            IBattleCard card = bundle.usedCards[i];
            if(card.CanUseCard(spawnPos) == false) { continue; }

            card.UseCard(spawnPos);
            usedCount++;
            int cardCost = card.GetSO().cost;
            usedCost += cardCost;
            usedCardNames.Add(card.GetSO().name);
            RegisterCardSpendForUpgradeScore(cardCost);
        }

        if(usedCount > 0) {
            history.Push(new UseCardTrace {
                cardName = usedCardNames,
                lane = bundle.lane,
                cost = usedCost,
                actionScore = bundle.score,
                resourceBefore = resourceBeforeBurst,
                resourceAfter = BattleInfo.teamDict[ourTeam].resource,
            });

            enemiesLeftToUpgrade = Mathf.Max(0, enemiesLeftToUpgrade - usedCount);
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
}