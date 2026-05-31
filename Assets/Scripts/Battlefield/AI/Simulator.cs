using System.Collections.Generic;
using UnityEngine;

public static class Simulator {
	private const float DamageDivisor = 7f;
	private const float SimulationStep = 1f;
	private const float MaxPostSpawnSeconds = 20f;
	private const float ResolveEpsilon = 0.0001f;
	private static List<SimEntity> world = new();

	private sealed class SimEntity {
		public Team team;
		public float x;
		public float moveSpeed;
		public float range;
		public float danger;
		public float survivability;

		public bool IsAlive => survivability > ResolveEpsilon;
	}

	public static float EvaluateBundle(IEntity[] laneEntities, IReadOnlyList<IBattleCard> spawnedCards, Team ourTeam, Team enemyTeam, int gridWidth, float lookahead) {
		List<SimEntity> laneSnapshots = SnapshotEntities(laneEntities);
		List<SimEntity> spawnedSnapshots = SnapshotCards(spawnedCards, ourTeam, gridWidth);
		return EvaluateBundle_2(laneSnapshots, spawnedSnapshots, ourTeam, enemyTeam, gridWidth, lookahead);
	}

	private static List<SimEntity> SnapshotEntities(IEntity[] laneEntities) {
		if(laneEntities == null || laneEntities.Length == 0) {
			return new List<SimEntity>(0);
		}

		List<SimEntity> list = new(laneEntities.Length);
		for(int i = 0; i < laneEntities.Length; ++i) {
			IEntity entity = laneEntities[i];
			if(entity == null) {
				continue;
			}

			list.Add(new SimEntity {
				team = entity.team,
				x = entity.gridPos.x,
				moveSpeed = Mathf.Max(0f, entity.GetRealMoveSpeed()),
				range = Mathf.Max(0f, entity[ST.Range]),
				danger = Mathf.Max(0f, entity.GetAssessPoint(APType.Danger)),
				survivability = Mathf.Max(0f, entity.GetAssessPoint(APType.Defend)),
			});
		}

		return list;
	}

	private static List<SimEntity> SnapshotCards(IReadOnlyList<IBattleCard> spawnedCards, Team ourTeam, int gridWidth) {
		if(spawnedCards == null || spawnedCards.Count == 0) {
			return new List<SimEntity>(0);
		}

		float spawnX = ourTeam == Team.Left ? 0f : gridWidth - 1;
		List<SimEntity> list = new(spawnedCards.Count);
		for(int i = 0; i < spawnedCards.Count; ++i) {
			IBattleCard card = spawnedCards[i];
			if(card == null) {
				continue;
			}

			EntitySO entitySO = card.GetSO()?.entitySO;
			// [Wrapper] Phase 4: read from prefab EntityWrapper for more accurate assess points
			if(entitySO == null) {
				continue;
			}

			// Estimate SimEntity values from SO (rough approximation — equivalent to level 1 with no effects)
			float moveSpeed = entitySO.moveSpeed / 4f;
			float range = entitySO.attackRange;
			float damage = entitySO.damage;
			float attackSpeed = entitySO.attackSpeed;
			float health = entitySO.health;
			float dangerEstimate = (0.9f + range / 10f) * attackSpeed * damage;
			float survivabilityEstimate = health / 7f + (dangerEstimate * 0f /* lifeSteal */ / 100f);

			list.Add(new SimEntity {
				team = ourTeam,
				x = spawnX,
				moveSpeed = Mathf.Max(0f, moveSpeed),
				range = Mathf.Max(0f, range),
				danger = Mathf.Max(0f, dangerEstimate),
				survivability = Mathf.Max(0f, survivabilityEstimate),
			});
		}

		return list;
	}

	private static float EvaluateBundle_2(List<SimEntity> laneEntities, List<SimEntity> spawnedCards, Team ourTeam, Team enemyTeam, int gridWidth, float lookahead) {
		ResetWorld();
		if(laneEntities != null) {
			world.AddRange(CloneEntities(laneEntities));
		}

		Simulate(gridWidth, lookahead, false);

		if(spawnedCards != null) {
			world.AddRange(CloneEntities(spawnedCards));
		}

		Simulate(gridWidth, MaxPostSpawnSeconds, true);
		return ScoreWorld(ourTeam, enemyTeam);
	}

	private static void ResetWorld() {
		world.Clear();
	}

	private static void Simulate(int gridWidth, float duration, bool stopWhenResolved) {
		float elapsed = 0f;
		while(elapsed < duration && (!stopWhenResolved || !IsResolved(world))) {
			float step = Mathf.Min(SimulationStep, duration - elapsed);
			Advance(gridWidth, step);
			elapsed += step;
		}
	}

	private static void Advance(int gridWidth, float step) {
		if(step <= 0f || world.Count == 0) {
			return;
		}

		for(int i = 0; i < world.Count; ++i) {
			SimEntity entity = world[i];
			if(!entity.IsAlive) {
				continue;
			}

			float direction = entity.team == Team.Left ? 1f : -1f;
			entity.x = Mathf.Clamp(entity.x + direction * entity.moveSpeed * step, 0f, gridWidth - 1);
		}

		float[] pendingDamage = new float[world.Count];
		for(int i = 0; i < world.Count; ++i) {
			SimEntity attacker = world[i];
			if(!attacker.IsAlive) {
				continue;
			}

			int targetIndex = FindNearestEnemy(world, attacker);
			if(targetIndex < 0) {
				continue;
			}

			SimEntity target = world[targetIndex];
			float distance = Mathf.Abs(attacker.x - target.x);
			if(distance > attacker.range) {
				continue;
			}

			pendingDamage[targetIndex] += Mathf.Max(0f, attacker.danger) / DamageDivisor * step;
		}

		for(int i = 0; i < world.Count; ++i) {
			SimEntity entity = world[i];
			if(!entity.IsAlive) {
				continue;
			}

			entity.survivability = Mathf.Max(0f, entity.survivability - pendingDamage[i]);
		}

		world.RemoveAll(entity => entity.survivability <= ResolveEpsilon);
	}

	private static List<SimEntity> CloneEntities(List<SimEntity> source) {
		List<SimEntity> cloned = new(source.Count);
		for(int i = 0; i < source.Count; ++i) {
			SimEntity entity = source[i];
			cloned.Add(new SimEntity {
				team = entity.team,
				x = entity.x,
				moveSpeed = entity.moveSpeed,
				range = entity.range,
				danger = entity.danger,
				survivability = entity.survivability,
			});
		}

		return cloned;
	}

	private static int FindNearestEnemy(List<SimEntity> world, SimEntity attacker) {
		int bestIndex = -1;
		float bestDistance = float.PositiveInfinity;

		for(int i = 0; i < world.Count; ++i) {
			SimEntity candidate = world[i];
			if(candidate.team == attacker.team || !candidate.IsAlive) {
				continue;
			}

			float distance = Mathf.Abs(candidate.x - attacker.x);
			if(distance < bestDistance) {
				bestDistance = distance;
				bestIndex = i;
			}
		}

		return bestIndex;
	}

	private static bool IsResolved(List<SimEntity> world) {
		bool hasOurTeam = false;
		bool hasEnemyTeam = false;

		for(int i = 0; i < world.Count; ++i) {
			SimEntity entity = world[i];
			if(!entity.IsAlive) {
				continue;
			}

			if(entity.team == Team.Right) {
				hasOurTeam = true;
			}
			else if(entity.team == Team.Left) {
				hasEnemyTeam = true;
			}

			if(hasOurTeam && hasEnemyTeam) {
				return false;
			}
		}

		return !(hasOurTeam && hasEnemyTeam);
	}

	private static float ScoreWorld(Team ourTeam, Team enemyTeam) {
		float ourPower = CalculateTeamPower(world, ourTeam);
		float enemyPower = CalculateTeamPower(world, enemyTeam);
		float score = ourPower - enemyPower;

		int ourAlive = CountAlive(world, ourTeam);
		int enemyAlive = CountAlive(world, enemyTeam);
		if(ourAlive > 0 && enemyAlive == 0) {
			score += 1000f;
		}
		else if(enemyAlive > 0 && ourAlive == 0) {
			score -= 1000f;
		}

		return score;
	}

	private static float CalculateTeamPower(List<SimEntity> world, Team team) {
		float dangerSum = 0f;
		float survivabilitySum = 0f;
		int unitCount = 0;

		for(int i = 0; i < world.Count; ++i) {
			SimEntity entity = world[i];
			if(entity.team != team || !entity.IsAlive) {
				continue;
			}

			unitCount++;
			dangerSum += entity.danger;
			survivabilitySum += entity.survivability;
		}

		if(unitCount == 0) {
			return 0f;
		}

		return dangerSum * survivabilitySum * TeamSnapshot.GetUnitCountDebuff(unitCount);
	}

	private static List<SimEntity> GetWorldSnapshot() {
		return CloneEntities(world);
	}

	private static int CountAlive(List<SimEntity> world, Team team) {
		int count = 0;
		for(int i = 0; i < world.Count; ++i) {
			SimEntity entity = world[i];
			if(entity.team == team && entity.IsAlive) {
				count++;
			}
		}

		return count;
	}
}
