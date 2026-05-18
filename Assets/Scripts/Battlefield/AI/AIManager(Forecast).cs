using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EnemyManager {
    /// <summary>
    /// biến này là biến tạm để các hàm BuildTeamForecastPoint, ComputeTeamSnapshot, GetAttackPosition 
    /// có thể truy cập được đánh giá về lane hiện tại mà không cần phải truyền qua tham số
    /// </summary>
    public LaneAssessment ans = new();
    public int specifiedLane, specifiedLookAhead;
    private List<LaneAssessment> laneAssessmentDetails = new();
    [ContextMenu("GetAssessment")]
    void GetAssessment() {
        laneAssessmentDetails.Clear();
        GetAssessment(specifiedLane, specifiedLookAhead);
    }
    /// <summary>
    /// Hàm này sẽ xây dựng kết quả xoay quanh field ansAssessment
    /// trong implementation sẽ gọi nhiều hàm con, những hàm con cũng truy cập thay đổi dữ liệu của field ansAssessment
    /// </summary>
    LaneAssessment GetAssessment(int lane, float lookAhead = 0) {
        if(lookAhead < 0) {
            Debug.LogError("[EnemyManager::GetCurrentPlayerPressure] Invalid parameter lookAhead");
            lookAhead = 0;
        }
        // Tìm trong cache xem có sẵn không, nếu có sẵn thì return
        // Còn không thì bắt đầu quấ trình tính toán heuristic
        foreach(LaneAssessment assessment in laneAssessmentDetails) {
            if(assessment.lane == lane && assessment.lookAhead == lookAhead) {
                return assessment;
            }
        }

        ans = new() {
            lane = lane,
            lookAhead = lookAhead
        };

        var entities = IEntityRegistry.Ins.GetEntitiesByLane(lane);

        foreach(IEntity e in entities) {
            ans[e.team].unitCount++;
        }

        BuildTeamFocusPosition(entities, Team.Left);
        BuildTeamFocusPosition(entities, Team.Right);
        EstimateTimeToFirstContact();

        bool willFightBeforeLookAhead = ans[ourTeam].unitCount > 0 && ans[o_Team].unitCount > 0 && ans.firstTimeToContact <= lookAhead;
        float combatDuration = Mathf.Max(0f, lookAhead - ans.firstTimeToContact);
        float opponentAliveRatio = 0, ourAliveRatio = 0;

        ComputeTeamSnapshotFromFocus(entities, Mathf.Min(lookAhead, ans.firstTimeToContact));

        if(willFightBeforeLookAhead) {
            
            float projectedDamageToOur = ans[o_Team].danger * combatDuration/6.6f;
            float projectedDamageToOppo = ans[ourTeam].danger * combatDuration/6.6f;

            float afterCombatSur = Mathf.Max(0f, ans[ourTeam].survivability - projectedDamageToOur);
            float afterCombatOpponentSur = Mathf.Max(0f, ans[o_Team].survivability - projectedDamageToOppo);

            opponentAliveRatio = afterCombatOpponentSur / ans[o_Team].survivability;
            ourAliveRatio = afterCombatSur / ans[ourTeam].survivability;
            ans[o_Team].danger *= Mathf.Lerp(1f, opponentAliveRatio, 0.4f);
            ans[ourTeam].danger *= Mathf.Lerp(1f, ourAliveRatio, 0.4f);
            ans[o_Team].survivability *= opponentAliveRatio;
            ans[ourTeam].survivability *= ourAliveRatio;
        }

        for(int i = 0; i < entities.Length; ++i) {
            IEntity e = entities[i];
            float projectedHpRatio = e.GetHealthPercentage() * (e.team == ourTeam ? ourAliveRatio : opponentAliveRatio);
            float hpWeight = 1f + (1f - projectedHpRatio) * 4f;
            ans[e.team].totalNeedProtection += e.GetAssessPoint(APType.NeedProtection) * hpWeight;
        }
        // float projectedEnemyHpRatio = snapshot[ourTeam].survivability > 0f ? enemySurvivability / snapshot[ourTeam].survivability : 0f;
        // float hpWeight = 1f + (1f - projectedEnemyHpRatio) * 4f;
        laneAssessmentDetails.Add(ans);
        return ans;
    }

    /// <summary>
    /// Build weighted focus points for one team.
    /// Weight = danger * survivability so stronger units pull focus more.
    /// </summary>
    internal void BuildTeamFocusPosition(IEntity[] entities, Team team) {
        TeamSnapshot snapshotRef = ans[team];
        float attackWeightedSum = 0f;
        float defensiveWeightedSum = 0f;

        float attackSum = 0f, defensiveSum = 0f;

        for(int i = 0; i < entities.Length; ++i) {
            IEntity e = entities[i];
            if(e.team != team) {
                continue;
            }

            float danger = Mathf.Max(0f, e.GetAssessPoint(APType.Danger));
            float survivability = Mathf.Max(0f, e.GetAssessPoint(APType.Defend));
            float power = Mathf.Max(1f, danger * survivability);

            attackWeightedSum += GetAttackPosition(e) * danger;
            defensiveWeightedSum += e.gridPos.x * survivability;
            attackSum += danger;
            defensiveSum += survivability;
        }
        
        snapshotRef.attackFocusPoint = attackWeightedSum / attackSum;
        snapshotRef.defensiveFocusPoint = defensiveWeightedSum / defensiveSum;
    }

    internal void ComputeTeamSnapshotFromFocus(IEntity[] entities, float timeTillContact)
    {
        for (int i = 0; i < entities.Length; ++i)
        {
            IEntity e = entities[i];
            float defensiveFocus = ans[e.team].defensiveFocusPoint;
            if(float.IsNaN(defensiveFocus)) {
                continue;
            }

            float attackPosition = GetAttackPosition(e) + timeTillContact * e.GetRealMoveSpeed() * 0.7f;
            float diff = Mathf.Abs(attackPosition - defensiveFocus);
            bool isBehindTank = e.team == Team.Left
                ? attackPosition <= defensiveFocus
                : attackPosition >= defensiveFocus;

            float dangerDistanceFactor;
            if(diff <= 1f) {
                dangerDistanceFactor = 1f;
            }
            else if(isBehindTank) {
                dangerDistanceFactor = Mathf.Clamp01(1f - diff / (IGrid.Ins.width * 2f));
            }
            else {
                dangerDistanceFactor = Mathf.Pow(Mathf.Clamp01(1f - diff / (IGrid.Ins.width * 1.2f)), 2f);
            }

            float defendDistanceFactor = Mathf.Lerp(1f, dangerDistanceFactor, 0.35f);
            ans[e.team].danger += e.GetAssessPoint(APType.Danger) * dangerDistanceFactor;
            ans[e.team].survivability += e.GetAssessPoint(APType.Defend) * defendDistanceFactor;
        }
    }

    internal void EstimateTimeToFirstContact() {
        IEntity[] entities = IEntityRegistry.Ins.GetEntitiesByLane(ans.lane);
        float ourSpeedSum = 0f;
        int ourCount = ans[ourTeam].unitCount;
        float opponentSpeedSum = 0f;
        int opponentCount = ans[o_Team].unitCount;

        for(int i = 0; i < entities.Length; ++i) {
            IEntity e = entities[i];
            float moveSpeed = e.GetRealMoveSpeed();
            if(e.team == ourTeam) {
                ourSpeedSum += moveSpeed;
            }
            else if(e.team == o_Team) {
                opponentSpeedSum += moveSpeed;
            }
        }

        if(ourCount == 0 || opponentCount == 0) {
            return;
        }

        float ourAverageSpeed = ourSpeedSum / ourCount;
        float opponentAverageSpeed = opponentSpeedSum / opponentCount;
        ans[o_Team].avgMoveSpeed = opponentAverageSpeed;
        ans[ourTeam].avgMoveSpeed = ourAverageSpeed;
        float approachingSpeed = ourAverageSpeed + opponentAverageSpeed;
        if(approachingSpeed <= 0f) {
            return;
        }

        float distanceFromOppo = Mathf.Abs(ans[o_Team].attackFocusPoint - ans[ourTeam].defensiveFocusPoint);
        float distanceFromOur = Mathf.Abs(ans[ourTeam].attackFocusPoint - ans[o_Team].defensiveFocusPoint);

        ans.firstTimeToContact = Mathf.Min(distanceFromOppo, distanceFromOur) / approachingSpeed;
    }
    internal static float GetAttackPosition(IEntity entity) {
        float range = Mathf.Max(0f, entity[ST.Range]);
        return entity.team == Team.Left ? entity.gridPos.x + range : entity.gridPos.x - range;
    }
    IEntity FindLaneLeader(IEntity[] entities, Team team) {
        float farthest = float.MinValue;
        IEntity leader = null;
        for(int i = 0; i < entities.Length; ++i) {
            IEntity e = entities[i];
            if(e.team != team) { continue; }

            float distanceScore = team == Team.Left ? e.DistanceToBase() : e.DistanceToOpponentBase();
            if(distanceScore > farthest) {
                farthest = distanceScore;
                leader = e;
            }
        }
        return leader;
    }

    float PredictFutureHealthPercentage(IEntity protectedEntity, IEntity[] opponents, float lookAhead) {
        float currentHpPercent = Mathf.Clamp01(protectedEntity.GetHealthPercentage());
        if(lookAhead <= 0f) {
            return currentHpPercent;
        }

        float incomingDangerPressure = 0f;
        for(int i = 0; i < opponents.Length; ++i) {
            IEntity opponent = opponents[i];
            if(opponent.team != Team.Left) { continue; }

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
    float EstimateApproachTime(TeamSnapshot o_assessment, IEntity target) {
        float closeDistance = Mathf.Abs(o_assessment.defensiveFocusPoint - target.gridPos.x);
        return closeDistance / o_assessment.avgMoveSpeed;
    }
}
