using System.Collections.Generic;
using UnityEngine;

public class SimulationResult {
	public float score;
	public List<Entity> remainings;
	public float ourPower;
	public float enemyPower;
}

public static class Simulator {
	private const float SimulationStep = 1f;
	private const float MaxPostSpawnSeconds = 10f;
	public static float ProximityMaxDistance = 10f;
	public static float ProximityPenaltyWeight = 50f;
	public static float ProtectionLossWeight = 10f;
	private static List<Entity> world = new();

	public static SimulationResult EvaluateBundle(IEntity[] allEntities, IReadOnlyList<IBattleCard> spawnedCards, int targetLane, Team ourTeam, Team enemyTeam, int gridWidth, float lookahead) {
		List<Entity> laneSnapshots = CloneEntities(allEntities);
		List<Entity> spawnedSnapshots = SnapshotCards(spawnedCards, ourTeam, gridWidth, targetLane);
		return EvaluateBundle_2(laneSnapshots, spawnedSnapshots, ourTeam, enemyTeam, gridWidth, lookahead);
	}

	private static List<Entity> SnapshotCards(IReadOnlyList<IBattleCard> spawnedCards, Team ourTeam, int gridWidth, int targetLane) {
		if(spawnedCards == null || spawnedCards.Count == 0) {
			return new List<Entity>(0);
		}

		float spawnX = ourTeam == Team.Left ? 0f : gridWidth - 1;
		List<Entity> list = new(spawnedCards.Count);
		for(int i = 0; i < spawnedCards.Count; ++i) {
			IBattleCard card = spawnedCards[i];
			if(card == null) {
				continue;
			}

			EntitySO entitySO = card.GetSO()?.entitySO;
			if(entitySO == null) {
				continue;
			}

			Entity clone = new Entity(entitySO, ourTeam, spawnX, targetLane, 1, true);
			list.Add(clone);
		}

		return list;
	}

	private static SimulationResult EvaluateBundle_2(List<Entity> laneEntities, List<Entity> spawnedCards, Team ourTeam, Team enemyTeam, int gridWidth, float lookahead) {
		bool hadInitialEnemies = false;
		bool hadInitialAllies = false;
		float initialProtection = 0f;

		if (laneEntities != null) {
			for (int i = 0; i < laneEntities.Count; i++) {
				if (laneEntities[i].team == enemyTeam) hadInitialEnemies = true;
				if (laneEntities[i].team == ourTeam) {
					hadInitialAllies = true;
					initialProtection += laneEntities[i].GetAssessPoint(APType.NeedProtection);
				}
			}
		}
		if (spawnedCards != null && spawnedCards.Count > 0) {
			hadInitialAllies = true;
		}

		ResetWorld();
		if(laneEntities != null) {
			world.AddRange(laneEntities);
		}

		Simulate(gridWidth, lookahead, false);

		if(spawnedCards != null) {
			world.AddRange(spawnedCards);
		}

		Simulate(gridWidth, MaxPostSpawnSeconds, true);
		return ScoreWorld(ourTeam, enemyTeam, hadInitialEnemies, hadInitialAllies, gridWidth, initialProtection);
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
			Entity entity = world[i];
			if(entity.IsDead()) {
				continue;
			}

			float direction = entity.team == Team.Left ? 1f : -1f;
			entity.gridPos.x = Mathf.Clamp(entity.gridPos.x + direction * entity.GetRealMoveSpeed() * step, 0f, gridWidth - 1);
		}

		float[] pendingDamage = new float[world.Count];
		for(int i = 0; i < world.Count; ++i) {
			Entity attacker = world[i];
			if(attacker.IsDead()) {
				continue;
			}

			int targetIndex = FindNearestEnemy(world, attacker);
			if(targetIndex < 0) {
				continue;
			}

			Entity target = world[targetIndex];
			float distance = Mathf.Abs(attacker.gridPos.x - target.gridPos.x);
			if(distance > attacker[ST.Range]) {
				continue;
			}

			pendingDamage[targetIndex] += Mathf.Max(0f, attacker.GetAssessPoint(APType.Danger)) * step;
		}

		for(int i = 0; i < world.Count; ++i) {
			Entity entity = world[i];
			if(entity.IsDead()) {
				continue;
			}

			if(pendingDamage[i] > 0) {
				entity.Stats[ST.Health] = Mathf.Max(0f, entity.Stats[ST.Health] - pendingDamage[i]);
				if(entity.Stats[ST.Health] <= 0f) {
					entity.Die();
				}
			}
		}

		world.RemoveAll(entity => entity.IsDead());
	}

	private static List<Entity> CloneEntities(IEnumerable<IEntity> source) {
		if (source == null) return new List<Entity>(0);
		List<Entity> cloned = new();
		foreach(var entity in source) {
			if(entity == null) continue;
			Entity clone = new Entity(entity.so, entity.team, entity.gridPos.x, entity.lane, entity.level, true);
			clone.Stats[ST.Health] = entity.Stats[ST.Health];
			cloned.Add(clone);
		}
		
		return cloned;
	}

	private static int FindNearestEnemy(List<Entity> world, Entity attacker) {
		int bestIndex = -1;
		float bestDistance = float.PositiveInfinity;

		for(int i = 0; i < world.Count; ++i) {
			Entity candidate = world[i];
			if(candidate.team == attacker.team || candidate.IsDead() || candidate.lane != attacker.lane) {
				continue;
			}

			float distance = Mathf.Abs(candidate.gridPos.x - attacker.gridPos.x);
			if(distance < bestDistance) {
				bestDistance = distance;
				bestIndex = i;
			}
		}

		return bestIndex;
	}

	private static bool IsResolved(List<Entity> world) {
		bool hasOurTeam = false;
		bool hasEnemyTeam = false;

		for(int i = 0; i < world.Count; ++i) {
			Entity entity = world[i];
			if(entity.IsDead()) {
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

	private static SimulationResult ScoreWorld(Team ourTeam, Team enemyTeam, bool hadInitialEnemies, bool hadInitialAllies, int gridWidth, float initialProtection) {
		float ourPower = CalculateTeamPower(world, ourTeam);
		float enemyPower = CalculateTeamPower(world, enemyTeam);
		float score = ourPower - enemyPower;

		// int ourAlive = CountAlive(world, ourTeam);
		// int enemyAlive = CountAlive(world, enemyTeam);
		// if(hadInitialEnemies && ourAlive > 0 && enemyAlive == 0) {
		// 	score += 1000f;
		// }
		// else if(hadInitialAllies && enemyAlive > 0 && ourAlive == 0) {
		// 	score -= 1000f;
		// }

		// Proximity penalty for enemies close to our base
		// float proximityPenalty = 0f;
		// for(int i = 0; i < world.Count; ++i) {
		// 	Entity entity = world[i];
		// 	if(entity.team == enemyTeam && !entity.IsDead()) {
		// 		float distanceToOurBase = (enemyTeam == Team.Left) ? (gridWidth - 1 - entity.gridPos.x) : entity.gridPos.x;
		// 		proximityPenalty += Mathf.Max(0f, ProximityMaxDistance - distanceToOurBase) * ProximityPenaltyWeight;
		// 	}
		// }
		// score -= proximityPenalty;

		// Protection loss penalty for our team
		float finalProtection = 0f;
		for(int i = 0; i < world.Count; ++i) {
			Entity entity = world[i];
			if(entity.team == ourTeam && !entity.IsDead()) {
				float hpPercent = entity.GetHealthPercentage();
				finalProtection += entity.GetAssessPoint(APType.NeedProtection) * hpPercent;
			}
		}
		float protectionLoss = Mathf.Max(0f, initialProtection - finalProtection);
		score -= protectionLoss * ProtectionLossWeight;

		return new SimulationResult {
			score = score,
			remainings = new List<Entity>(world),
			ourPower = ourPower,
			enemyPower = enemyPower
		};
	}

	private static float CalculateTeamPower(List<Entity> world, Team team) {
		float answer = 0f;
		for(int i = 0; i < world.Count; ++i) {
			Entity entity = world[i];
			if(entity.team != team || entity.IsDead()) {
				continue;
			}
			answer += entity.GetAssessPoint(APType.Danger) * entity.GetAssessPoint(APType.Defend);
		}
		return answer;
	}

	private static int CountAlive(List<Entity> world, Team team) {
		int count = 0;
		for(int i = 0; i < world.Count; ++i) {
			Entity entity = world[i];
			if(entity.team == team && !entity.IsDead()) {
				count++;
			}
		}

		return count;
	}
}
