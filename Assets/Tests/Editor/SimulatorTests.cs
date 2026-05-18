using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class SimulatorTests {
	[Test]
	public void Simulator_EmptyWorld_Logs() {
		List<Simulator.FakeEntity> lane = new List<Simulator.FakeEntity>();
		List<Simulator.FakeEntity> spawned = new List<Simulator.FakeEntity>();
		float score = Simulator.EvaluateBundle_2(lane, spawned, Team.Left, Team.Right, 12, 0f);
		Debug.Log("[EmptyWorld] Score: " + score);
		var final = Simulator.GetWorldSnapshot();
		Debug.Log("[EmptyWorld] Final world count: " + final.Count);
		foreach(var e in final) {
			Debug.Log($"[EmptyWorld] Entity team={e.team} x={e.x} danger={e.danger} surv={e.survivability}");
		}
		Assert.Pass();
	}

	[Test]
	public void Simulator_SingleSpawnVsEnemy_Logs() {
		// Enemy already on lane at x=6
		var lane = new List<Simulator.FakeEntity> {
			new Simulator.FakeEntity { team = Team.Right, x = 6f, moveSpeed = 1f, range = 1f, danger = 25, survivability = 30 }
		};

		// Our spawn (will be added at provided x by EvaluateBundle_2)
		var spawned = new List<Simulator.FakeEntity> {
			new Simulator.FakeEntity { team = Team.Left, x = 0f, moveSpeed = 1f, range = 1f, danger = 30, survivability = 30f }
		};

		float score = Simulator.EvaluateBundle_2(lane, spawned, Team.Left, Team.Right, 12, 0f);
		Debug.Log("[SingleSpawn] Score: " + score);
		var final = Simulator.GetWorldSnapshot();
		Debug.Log("[SingleSpawn] Final world count: " + final.Count);
		foreach(var e in final) {
			Debug.Log($"[SingleSpawn] Entity team={e.team} x={e.x} danger={e.danger} surv={e.survivability}");
		}
		Assert.Pass();
	}

	[Test]
	public void Simulator_MultiUnitScenario_Grid15_Logs() {
		var lane = new List<Simulator.FakeEntity> {
			new Simulator.FakeEntity { team = Team.Right, x = 10f, moveSpeed = 1.2f, range = 1f, danger = 6f, survivability = 12f },
			new Simulator.FakeEntity { team = Team.Right, x = 11f, moveSpeed = 0.8f, range = 1f, danger = 4f, survivability = 8f }
		};

		var spawned = new List<Simulator.FakeEntity> {
			new Simulator.FakeEntity { team = Team.Left, x = 0f, moveSpeed = 1f, range = 1f, danger = 7f, survivability = 9f },
			new Simulator.FakeEntity { team = Team.Left, x = 0f, moveSpeed = 1.5f, range = 1f, danger = 3f, survivability = 6f }
		};

		float score = Simulator.EvaluateBundle_2(lane, spawned, Team.Left, Team.Right, 15, 0f);
		Debug.Log("[MultiUnit] Score: " + score);
		var final = Simulator.GetWorldSnapshot();
		Debug.Log("[MultiUnit] Final world count: " + final.Count);
		foreach(var e in final) {
			Debug.Log($"[MultiUnit] Entity team={e.team} x={e.x} danger={e.danger} surv={e.survivability}");
		}
		Assert.Pass();
	}
}
