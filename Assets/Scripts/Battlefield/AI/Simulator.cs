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
	private const float MaxPostSpawnSeconds = 20f;
	private static List<Entity> world = new();

	public static SimulationResult EvaluateBundle(IEntity[] laneEntities, IReadOnlyList<IBattleCard> spawnedCards, Team ourTeam, Team enemyTeam, int gridWidth, float lookahead) {
		List<Entity> laneSnapshots = CloneEntities(laneEntities);
		List<Entity> spawnedSnapshots = SnapshotCards(spawnedCards, ourTeam, gridWidth);
		return EvaluateBundle_2(laneSnapshots, spawnedSnapshots, ourTeam, enemyTeam, gridWidth, lookahead);
	}

	private static List<Entity> SnapshotCards(IReadOnlyList<IBattleCard> spawnedCards, Team ourTeam, int gridWidth) {
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

			Entity clone = new Entity(entitySO, ourTeam, spawnX, 0, 1, true);
			list.Add(clone);
		}

		return list;
	}

	private static SimulationResult EvaluateBundle_2(List<Entity> laneEntities, List<Entity> spawnedCards, Team ourTeam, Team enemyTeam, int gridWidth, float lookahead) {
		ResetWorld();
		if(laneEntities != null) {
			world.AddRange(laneEntities);
		}

		Simulate(gridWidth, lookahead, false);

		if(spawnedCards != null) {
			world.AddRange(spawnedCards);
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
			if(candidate.team == attacker.team || candidate.IsDead()) {
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

	private static SimulationResult ScoreWorld(Team ourTeam, Team enemyTeam) {
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

		return new SimulationResult {
			score = score,
			remainings = new List<Entity>(world),
			ourPower = ourPower,
			enemyPower = enemyPower
		};
	}

	private static float CalculateTeamPower(List<Entity> world, Team team) {
		float dangerSum = 0f;
		float survivabilitySum = 0f;
		int unitCount = 0;

		for(int i = 0; i < world.Count; ++i) {
			Entity entity = world[i];
			if(entity.team != team || entity.IsDead()) {
				continue;
			}

			unitCount++;
			dangerSum += entity.GetAssessPoint(APType.Danger);
			survivabilitySum += entity.GetAssessPoint(APType.Defend);
		}

		if(unitCount == 0) {
			return 0f;
		}

		return dangerSum * survivabilitySum * TeamSnapshot.GetUnitCountDebuff(unitCount);
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
