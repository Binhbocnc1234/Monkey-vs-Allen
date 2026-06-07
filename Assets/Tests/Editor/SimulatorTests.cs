using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class SimulatorTests {
	private EntitySO CreateMinimalEntitySO() {
		EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
		so.id = Guid.NewGuid().ToString();
		so.health = 100;
		so.canAttack = true;
		so.damage = 10;
		so.attackSpeed = 1f;
		so.moveSpeed = 2f;
		so.attackRange = 1;
		so.behaviourTemplates = new List<IBehaviour> {
			new MeleeAttack(),
			new StraightMove(),
		};
		return so;
	}

	private Entity CreateTestEntity(EntitySO so = null, Team team = Team.Left, float x = 0f, int lane = 0, int level = 1) {
		if(so == null) so = CreateMinimalEntitySO();
		return new Entity(so, team, x, lane, level, true);
	}

	private void LogResult(string testName, SimulationResult result) {
		Debug.Log($"[SimTest] {testName}: score={result.score:F2}, remainings={result.remainings.Count}, ourPower={result.ourPower:F2}, enemyPower={result.enemyPower:F2}");
		for(int i = 0; i < result.remainings.Count; ++i) {
			var e = result.remainings[i];
			Debug.Log($"  [{i}] team={e.team}, x={e.gridPos.x:F2}, hp={e.Stats[ST.Health]:F2}/{e.Stats[ST.MaxHealth]:F2}, danger={e.GetAssessPoint(APType.Danger):F2}");
		}
	}

	[Test]
	public void Test1_Empty_ReturnsZeroScore() {
		var result = Simulator.EvaluateBundle(new IEntity[0], null, Team.Left, Team.Right, 10, 20f);
		LogResult("Test1_Empty", result);
		Assert.AreEqual(0f, result.score);
		Assert.AreEqual(0, result.remainings.Count);
	}

	[Test]
	public void Test2_OneEntityInOneTeam_ReturnsPositiveScore() {
		Entity leftEntity = CreateTestEntity(null, Team.Left, 0f);
		var result = Simulator.EvaluateBundle(new IEntity[] { leftEntity }, null, Team.Left, Team.Right, 10, 20f);
		LogResult("Test2_OneEntity", result);
		
		Assert.Greater(result.score, 0f);
		Assert.AreEqual(1, result.remainings.Count);
		Assert.AreEqual(Team.Left, result.remainings[0].team);
	}

	[Test]
	public void Test3_OneVsOne_Identical_ShouldBeFair() {
		EntitySO so = CreateMinimalEntitySO();
		Entity leftEntity = CreateTestEntity(so, Team.Left, 2f);
		Entity rightEntity = CreateTestEntity(so, Team.Right, 7f);
		
		var result = Simulator.EvaluateBundle(new IEntity[] { leftEntity, rightEntity }, null, Team.Left, Team.Right, 10, 20f);
		LogResult("Test3_1v1", result);
		
		// Symmetric: score should be near 0 (either both die or close result)
		Assert.Less(Mathf.Abs(result.score), 1500f); 
	}

	[Test]
	public void Test4_TwoVsOne_Identical_TeamAAdvantage() {
		EntitySO so = CreateMinimalEntitySO();
		Entity left1 = CreateTestEntity(so, Team.Left, 2f);
		Entity left2 = CreateTestEntity(so, Team.Left, 2f);
		Entity right1 = CreateTestEntity(so, Team.Right, 7f);

		var result = Simulator.EvaluateBundle(new IEntity[] { left1, left2, right1 }, null, Team.Left, Team.Right, 10, 20f);
		LogResult("Test4_2v1", result);

		// 2v1 should be positive for Left team
		Assert.Greater(result.score, 0f, "2v1 should favor the team with more units");
	}

	[Test]
	public void Test5_TwoVsTwo_Identical_ShouldBeFair() {
		EntitySO so = CreateMinimalEntitySO();
		Entity left1 = CreateTestEntity(so, Team.Left, 2f);
		Entity left2 = CreateTestEntity(so, Team.Left, 2f);
		Entity right1 = CreateTestEntity(so, Team.Right, 7f);
		Entity right2 = CreateTestEntity(so, Team.Right, 7f);

		var result = Simulator.EvaluateBundle(new IEntity[] { left1, left2, right1, right2 }, null, Team.Left, Team.Right, 10, 20f);
		LogResult("Test5_2v2", result);

		Assert.Less(Mathf.Abs(result.score), 1500f);
	}

	[Test]
	public void Test6_TwoVsTwo_PositionAdvantage_TeamABetter() {
		EntitySO so = CreateMinimalEntitySO();
		// Team Left grouped
		Entity left1 = CreateTestEntity(so, Team.Left, 1f);
		Entity left2 = CreateTestEntity(so, Team.Left, 1.1f);
		// Team Right staggered far apart
		Entity right1 = CreateTestEntity(so, Team.Right, 6f);
		Entity right2 = CreateTestEntity(so, Team.Right, 9f);

		var result = Simulator.EvaluateBundle(new IEntity[] { left1, left2, right1, right2 }, null, Team.Left, Team.Right, 10, 20f);
		LogResult("Test6_2v2_pos", result);

		// Left should have position advantage (grouped vs staggered)
		Assert.Greater(result.score, 0f, "Grouped team should beat staggered team");
	}
}
