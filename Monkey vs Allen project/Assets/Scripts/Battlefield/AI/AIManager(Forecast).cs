using System.Collections.Generic;
using UnityEngine;

public partial class EnemyManager {
    /// <summary>
    /// biến này là biến tạm để các hàm BuildTeamForecastPoint, ComputeTeamSnapshot, GetAttackPosition 
    /// có thể truy cập được đánh giá về lane hiện tại mà không cần phải truyền qua tham số
    /// </summary>
    public LaneAssessment ansAssess = new();
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

        ansAssess = new();

        var entities = IEntityRegistry.Ins.GetEntitiesByLane(lane);
        foreach(IEntity e in entities) {
            ansAssess[e.team].unitCount++;
        }

        BuildTeamForecastPoint(entities, Team.Left);
        BuildTeamForecastPoint(entities, Team.Right);

        ComputeTeamSnapshotFromFocus(entities);

        float timeToFirstContact = EstimateTimeToFirstContact(lane, ansAssess[ourTeam].attackFocusPoint, ansAssess[o_Team].attackFocusPoint);
        bool willFightBeforeLookAhead = ansAssess[o_Team].unitCount > 0 && timeToFirstContact <= lookAhead;
        float combatDuration = Mathf.Max(0f, lookAhead - timeToFirstContact);
        float opponentAliveRatio = 0, ourAliveRatio = 0;

        if(willFightBeforeLookAhead) {
            
            float projectedDamageToOur = ansAssess[o_Team].danger * combatDuration * ansAssess[o_Team].GetUnitCountDebuff();
            float projectedDamageToOppo = ansAssess[ourTeam].danger * combatDuration * ansAssess[o_Team].GetUnitCountDebuff();

            float afterCombatSur = Mathf.Max(0f, ansAssess[ourTeam].survivability - projectedDamageToOur);
            float afterCombatOpponentSur = Mathf.Max(0f, ansAssess[o_Team].survivability - projectedDamageToOppo);

            opponentAliveRatio = afterCombatOpponentSur / ansAssess[o_Team].survivability;
            ourAliveRatio = afterCombatSur / ansAssess[ourTeam].survivability;
            ansAssess[o_Team].danger *= opponentAliveRatio;
            ansAssess[ourTeam].danger *= ourAliveRatio;
            ansAssess[o_Team].survivability *= opponentAliveRatio;
            ansAssess[ourTeam].survivability *= ourAliveRatio;
        }

        for(int i = 0; i < entities.Length; ++i) {
            IEntity e = entities[i];
            float projectedHpRatio = e.GetHealthPercentage() * (e.team == ourTeam ? ourAliveRatio : opponentAliveRatio);
            float hpWeight = 1f + (1f - projectedHpRatio) * 4f;
            ansAssess[e.team].totalNeedProtection += e.GetAssessPoint(APType.NeedProtection) * hpWeight;
        }
        // float projectedEnemyHpRatio = snapshot[ourTeam].survivability > 0f ? enemySurvivability / snapshot[ourTeam].survivability : 0f;
        // float hpWeight = 1f + (1f - projectedEnemyHpRatio) * 4f;
        laneAssessmentDetails.Add(ansAssess);
        return ansAssess;
    }

    /// <summary>
    /// Build weighted focus points for one team.
    /// Weight = danger * survivability so stronger units pull focus more.
    /// </summary>
    internal void BuildTeamForecastPoint(IEntity[] entities, Team team) {
        TeamSnapshot snapshotRef = ansAssess[team];
        float actualWeightedSum = 0f;
        float attackWeightedSum = 0f;;
        float weightSum = 0f;

        for(int i = 0; i < entities.Length; ++i) {
            IEntity entity = entities[i];
            if(entity.team != team) {
                continue;
            }

            float danger = Mathf.Max(0f, entity.GetAssessPoint(APType.Danger));
            float survivability = Mathf.Max(0f, entity.GetAssessPoint(APType.Defend));
            float power = Mathf.Max(1f, danger * survivability);
            float actualPosition = entity.gridPos.x;
            float attackPosition = GetAttackPosition(entity);

            actualWeightedSum += actualPosition * power;
            attackWeightedSum += attackPosition * power;
            weightSum += power;
        }

        if(weightSum > 0f) {
            snapshotRef.actualFocusPoint = actualWeightedSum / weightSum;
            snapshotRef.attackFocusPoint = attackWeightedSum / weightSum;
        }
        else {
            snapshotRef.actualFocusPoint = float.NaN;
            snapshotRef.attackFocusPoint = float.NaN;
        }
    }

    internal void ComputeTeamSnapshotFromFocus(IEntity[] entities)
    {
        for (int i = 0; i < entities.Length; ++i)
        {
            IEntity e = entities[i];
            float attackPosition = GetAttackPosition(e);
            float distanceToFocus = Mathf.Abs(attackPosition - ansAssess[e.team].attackFocusPoint);
            float speedSafe = Mathf.Max(e[ST.MoveSpeed], 0.1f);
            float distanceFactor = 1f - distanceToFocus / speedSafe / 4f;
            if (distanceFactor < 0f)
            {
                continue;
            }
            ansAssess[e.team].danger += e.GetAssessPoint(APType.Danger) * distanceFactor;
            ansAssess[e.team].survivability += e.GetAssessPoint(APType.Defend) * distanceFactor;
        }
    }
    
    internal static float GetAttackPosition(IEntity entity) {
        float range = Mathf.Max(0f, entity[ST.Range]);
        return entity.team == Team.Left ? entity.gridPos.x + range*2/3 : entity.gridPos.x - range*2/3;
    }

    internal float EstimateTimeToFirstContact(int lane, float o_attackPoint, float attackFocusPoint) {
        if(float.IsNaN(attackFocusPoint) || float.IsNaN(o_attackPoint)) {
            return float.PositiveInfinity;
        }

        IEntity[] entities = IEntityRegistry.Ins.GetEntitiesByLane(lane);
        float ourSpeedSum = 0f;
        int ourCount = ansAssess[ourTeam].unitCount;
        float opponentSpeedSum = 0f;
        int opponentCount = ansAssess[o_Team].unitCount;

        for(int i = 0; i < entities.Length; ++i) {
            IEntity entity = entities[i];
            float moveSpeed = Mathf.Max(0f, entity[ST.MoveSpeed]);
            if(entity.team == Team.Left) {
                ourSpeedSum += moveSpeed;
            }
            else if(entity.team == Team.Right) {
                opponentSpeedSum += moveSpeed;
            }
        }

        if(ourCount == 0 || opponentCount == 0) {
            return float.PositiveInfinity;
        }

        float ourAverageSpeed = ourSpeedSum / ourCount;
        float opponentAverageSpeed = opponentSpeedSum / opponentCount;
        ansAssess[o_Team].avgMoveSpeed = opponentAverageSpeed;
        ansAssess[ourTeam].avgMoveSpeed = ourAverageSpeed;
        float approachingSpeed = ourAverageSpeed + opponentAverageSpeed;
        if(approachingSpeed <= 0f) {
            return float.PositiveInfinity;
        }

        float distance = Mathf.Abs(o_attackPoint - attackFocusPoint);
        return distance / approachingSpeed;
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
        float closeDistance = Mathf.Abs(o_assessment.attackFocusPoint - target.gridPos.x);
        return closeDistance / o_assessment.avgMoveSpeed;
    }
}
