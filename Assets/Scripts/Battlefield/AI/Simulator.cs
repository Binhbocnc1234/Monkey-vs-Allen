using System.Collections.Generic;
using UnityEngine;

public static class Simulator {
	private const float DamageDivisor = 7f;
	private const float SimulationStep = 1f;
	private const float MaxPostSpawnSeconds = 20f;
	private const float ResolveEpsilon = 0.0001f;
	private static List<FakeEntity> world = new();

	public sealed class FakeEntity {
		public Team team;
		public float x;
		public float moveSpeed;
		public float range;
		public float danger;
		public float survivability;

		public bool IsAlive => survivability > ResolveEpsilon;
	}

	public static float EvaluateBundle(IEntity[] laneEntities, IReadOnlyList<IBattleCard> spawnedCards, Team ourTeam, Team enemyTeam, int gridWidth, float lookahead) {
		List<FakeEntity> laneFakes = ConvertIEntitiesToFake(laneEntities);
		List<FakeEntity> spawnedFakes = ConvertCardsToFake(spawnedCards, ourTeam, gridWidth);
		return EvaluateBundle_2(laneFakes, spawnedFakes, ourTeam, enemyTeam, gridWidth, lookahead);
	}

	private static List<FakeEntity> ConvertIEntitiesToFake(IEntity[] laneEntities) {
		if(laneEntities == null || laneEntities.Length == 0) {
			return new List<FakeEntity>(0);
		}

		List<FakeEntity> list = new(laneEntities.Length);
		for(int i = 0; i < laneEntities.Length; ++i) {
			IEntity entity = laneEntities[i];
			if(entity == null) {
				continue;
			}

			list.Add(new FakeEntity {
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

	private static List<FakeEntity> ConvertCardsToFake(IReadOnlyList<IBattleCard> spawnedCards, Team ourTeam, int gridWidth) {
		if(spawnedCards == null || spawnedCards.Count == 0) {
			return new List<FakeEntity>(0);
		}

		float spawnX = ourTeam == Team.Left ? 0f : gridWidth - 1;
		List<FakeEntity> list = new(spawnedCards.Count);
		for(int i = 0; i < spawnedCards.Count; ++i) {
			IBattleCard card = spawnedCards[i];
			if(card == null) {
				continue;
			}

			IEntity prefab = card.GetSO()?.entitySO?.prefab?.GetComponent<IEntity>();
			if(prefab == null) {
				continue;
			}

			list.Add(new FakeEntity {
				team = ourTeam,
				x = spawnX,
				moveSpeed = Mathf.Max(0f, prefab.GetRealMoveSpeed()),
				range = Mathf.Max(0f, prefab[ST.Range]),
				danger = Mathf.Max(0f, prefab.GetAssessPoint(APType.Danger)),
				survivability = Mathf.Max(0f, prefab.GetAssessPoint(APType.Defend)),
			});
		}

		return list;
	}

	public static float EvaluateBundle_2(List<FakeEntity> laneEntities, List<FakeEntity> spawnedCards, Team ourTeam, Team enemyTeam, int gridWidth, float lookahead) {
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
			FakeEntity entity = world[i];
			if(!entity.IsAlive) {
				continue;
			}

			float direction = entity.team == Team.Left ? 1f : -1f;
			entity.x = Mathf.Clamp(entity.x + direction * entity.moveSpeed * step, 0f, gridWidth - 1);
		}

		float[] pendingDamage = new float[world.Count];
		for(int i = 0; i < world.Count; ++i) {
			FakeEntity attacker = world[i];
			if(!attacker.IsAlive) {
				continue;
			}

			int targetIndex = FindNearestEnemy(world, attacker);
			if(targetIndex < 0) {
				continue;
			}

			FakeEntity target = world[targetIndex];
			float distance = Mathf.Abs(attacker.x - target.x);
			if(distance > attacker.range) {
				continue;
			}

			pendingDamage[targetIndex] += Mathf.Max(0f, attacker.danger) / DamageDivisor * step;
		}

		for(int i = 0; i < world.Count; ++i) {
			FakeEntity entity = world[i];
			if(!entity.IsAlive) {
				continue;
			}

			entity.survivability = Mathf.Max(0f, entity.survivability - pendingDamage[i]);
		}

		world.RemoveAll(entity => entity.survivability <= ResolveEpsilon);
	}

	private static List<FakeEntity> CloneEntities(List<FakeEntity> source) {
		List<FakeEntity> cloned = new(source.Count);
		for(int i = 0; i < source.Count; ++i) {
			FakeEntity entity = source[i];
			cloned.Add(new FakeEntity {
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

	private static int FindNearestEnemy(List<FakeEntity> world, FakeEntity attacker) {
		int bestIndex = -1;
		float bestDistance = float.PositiveInfinity;

		for(int i = 0; i < world.Count; ++i) {
			FakeEntity candidate = world[i];
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

	private static bool IsResolved(List<FakeEntity> world) {
		bool hasOurTeam = false;
		bool hasEnemyTeam = false;

		for(int i = 0; i < world.Count; ++i) {
			FakeEntity entity = world[i];
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

	private static float CalculateTeamPower(List<FakeEntity> world, Team team) {
		float dangerSum = 0f;
		float survivabilitySum = 0f;
		int unitCount = 0;

		for(int i = 0; i < world.Count; ++i) {
			FakeEntity entity = world[i];
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

	public static List<FakeEntity> GetWorldSnapshot() {
		return CloneEntities(world);
	}

	private static int CountAlive(List<FakeEntity> world, Team team) {
		int count = 0;
		for(int i = 0; i < world.Count; ++i) {
			FakeEntity entity = world[i];
			if(entity.team == team && entity.IsAlive) {
				count++;
			}
		}

		return count;
	}
}