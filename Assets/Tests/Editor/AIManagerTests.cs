using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class MockBattleCard : IBattleCard {
	public static MockBattleCard Create(int cost, EntitySO entitySO) {
		var go = new GameObject("MockCard");
		var card = go.AddComponent<MockBattleCard>();
		card.cost = cost;
		card.originalCost = cost;
		card.cooldownTimer = new Timer(0f, false);
		card.effects = new List<CardEffect>();
		
		var enemySO = ScriptableObject.CreateInstance<EnemyCardSO>();
		enemySO.cost = cost;
		enemySO.entitySO = entitySO;
		enemySO.cooldownType = CoolDownType.Low;
		
		typeof(IBattleCard)
			.GetField("so", BindingFlags.NonPublic | BindingFlags.Instance)
			.SetValue(card, enemySO);
			
		return card;
	}

	public override bool CanUseCard(Vector2Int gridPosition) => true;
	public override void UseCard(Vector2Int gridPosition) {}
	public override void Update() {}
	public override SelectMessage CanSelectCard() => SelectMessage.CanSelect;
	public override void SetCoolDown(float newCoolDown) {}
}

public class AIManagerMockGrid : IGrid {
	public void Setup(int w, int h, bool[] open) {
		this.width = w;
		this.height = h;
		this.openLanes = open;
	}

	public override void Initialize(int width, bool[] openLanes) {}
	public override bool IsValidGridPosition(float x, float y) => true;
	public override Vector2 GridToWorldPosition(Vector2 gridPos) => Vector2.zero;
	public override Vector2 GridToWorldPosition(float x, float y) => Vector2.zero;
	public override Vector2 WorldToGridPos(Vector2 v) => Vector2.zero;
	public override Vector2Int WorldToGridPosRounded(Vector2 worldPosition) => Vector2Int.zero;
	public override ICell GetCell(Vector2Int gridPos) => null;
	public override ICell GetCell(int x, int y) => null;
	public override void CreateCell(ICell cellPrefab, int x, int y) {}
	public override void Clear() {}
	public override List<int> GetOpenLanes() {
		List<int> lanes = new List<int>();
		if (openLanes != null) {
			for (int i = 0; i < openLanes.Length; i++) {
				if (openLanes[i]) lanes.Add(i);
			}
		}
		return lanes;
	}
}

[TestFixture]
public class AIManagerTests {
	private GameObject gridGo;
	private GameObject containerGo;
	private EContainer container;
	private List<List<Entity>> entities;
	private LevelSO testLevelSO;
	private EntitySO mockEntitySO;
	private List<GameObject> tempGameObjects = new List<GameObject>();

	private EntitySO CreateTestEntitySO(int hp, int dmg) {
		EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
		so.id = Guid.NewGuid().ToString();
		so.health = hp;
		so.canAttack = true;
		so.damage = dmg;
		so.attackSpeed = 1f;
		so.moveSpeed = 1f;
		so.attackRange = 1;
		so.behaviourTemplates = new List<IBehaviour> {
			new MeleeAttack(),
			new StraightMove(),
		};
		
		GameObject dummy = new GameObject("DummyPrefab");
		tempGameObjects.Add(dummy);
		so.prefab = dummy;
		
		return so;
	}

	[SetUp]
	public void SetUp() {
		// 1. Mock Grid System
		gridGo = new GameObject("MockGrid");
		AIManagerMockGrid mockGrid = gridGo.AddComponent<AIManagerMockGrid>();
		mockGrid.Setup(10, 6, new bool[] { true, true, true, false, false, false }); // 3 open lanes, height 6
		typeof(Singleton<IGrid>)
			.GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)
			.SetValue(null, mockGrid);

		// 2. Mock EContainer (Entity Registry)
		containerGo = new GameObject("EContainer");
		container = containerGo.AddComponent<EContainer>();
		MethodInfo awake = typeof(Singleton<IEntityRegistry>)
			.GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
		awake.Invoke(container, null);

		var entitiesField = typeof(EContainer)
			.GetField("entities", BindingFlags.NonPublic | BindingFlags.Instance);
		entities = new List<List<Entity>>();
		for (int i = 0; i < 6; i++) {
			entities.Add(new List<Entity>());
		}
		entitiesField.SetValue(container, entities);

		// 3. Reset static BattleInfo
		BattleInfo.Reset();
		testLevelSO = ScriptableObject.CreateInstance<LevelSO>();
		testLevelSO.alienWaitingTime = 0f;
		testLevelSO.initialBanana = 100;
		testLevelSO.enemies = new List<EnemyCardSO>();
		testLevelSO.openLanes = new bool[] { true, true, true, false, false, false }; // 3 open lanes
		BattleInfo.Initialize(testLevelSO);

		// 4. Reset AI Singletons
		AIManager.ResetInstance();
		AlienResourceManager.ResetInstance();
		
		mockEntitySO = CreateTestEntitySO(100, 10);
		BattleInfo.ChangeState(GameState.Fighting);
	}

	[TearDown]
	public void TearDown() {
		// Clean up singletons
		typeof(Singleton<IGrid>)
			.GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)
			.SetValue(null, null);

		typeof(Singleton<IEntityRegistry>)
			.GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)
			.SetValue(null, null);

		if (gridGo != null) GameObject.DestroyImmediate(gridGo);
		if (containerGo != null) GameObject.DestroyImmediate(containerGo);

		foreach (var go in tempGameObjects) {
			if (go != null) GameObject.DestroyImmediate(go);
		}
		tempGameObjects.Clear();

		AIManager.ResetInstance();
		AlienResourceManager.ResetInstance();
		BattleInfo.ChangeState(GameState.ChoosingCard);
	}

	[Test]
	public void Test_CalculateLookaheadToAfford() {
		AIManager.Ins.Initialize();
		var method = typeof(AIManager).GetMethod("CalculateLookaheadToAfford", BindingFlags.NonPublic | BindingFlags.Instance);
		
		// Case A: we already have enough resources
		BattleInfo.teamDict[Team.Right].resource = 10;
		float lookahead = (float)method.Invoke(AIManager.Ins, new object[] { 5 });
		Assert.AreEqual(0f, lookahead);

		// Case B: not enough resources, calculate wait time
		AlienResourceManager.Ins.resourceTimer = new Timer(5f, true);
		AlienResourceManager.Ins.resourceTimer.remainingTime = 3f;
		AlienResourceManager.Ins.upgradeCnt = 1;
		
		BattleInfo.teamDict[Team.Right].resource = 10;
		// missing = 15 - 10 = 5.
		// BattleInfo.resourcePerGeneration is 30, so 1 tick is enough.
		// ticksNeeded = Ceil(5/30) = 1.
		// wait time = remainingTime + Max(0, ticks - 1) * interval = 3 + 0 = 3f.
		lookahead = (float)method.Invoke(AIManager.Ins, new object[] { 15 });
		Assert.AreEqual(3f, lookahead);
	}

	[Test]
	public void Test_UpgradeDecision_WhenSafe() {
		AIManager.Ins.Initialize();
		AIManager.Ins.enemiesLeftToUpgrade = 0;
		AIManager.Ins.upgradeLeft = 1;
		AlienResourceManager.Ins.costToUpgrade = 5;
		BattleInfo.teamDict[Team.Right].resource = 10;
		
		// Hand is empty
		BattleInfo.teamDict[Team.Right].cards.Clear();

		var method = typeof(AIManager).GetMethod("FindBestAction", BindingFlags.NonPublic | BindingFlags.Instance);
		AIAction bestAction = (AIAction)method.Invoke(AIManager.Ins, null);

		Assert.IsNotNull(bestAction);
		Assert.IsInstanceOf<UpgradeAction>(bestAction);
	}

	[Test]
	public void Test_SpawnDecision_EmergencyDefense() {
		AIManager.Ins.Initialize();
		AIManager.Ins.enemiesLeftToUpgrade = 0;
		AIManager.Ins.upgradeLeft = 1;
		AIManager.Ins.upgradeThreshold = 800f; // flat upgrade score
		AlienResourceManager.Ins.costToUpgrade = 5;
		BattleInfo.teamDict[Team.Right].resource = 10;

		typeof(AIManager)
			.GetField("firstUpgrade", BindingFlags.NonPublic | BindingFlags.Instance)
			.SetValue(AIManager.Ins, false);

		// 1. Add strong enemy close to our base (Team.Left is attacking, x is near base)
		Entity attacker = new Entity(mockEntitySO, Team.Left, 1f, 0, 1, true);
		entities[0].Add(attacker);

		// 2. Give AI a card on hand
		var card = MockBattleCard.Create(2, mockEntitySO);
		BattleInfo.teamDict[Team.Right].cards.Add(card);

		var method = typeof(AIManager).GetMethod("FindBestAction", BindingFlags.NonPublic | BindingFlags.Instance);
		AIAction bestAction = (AIAction)method.Invoke(AIManager.Ins, null);

		// AI should prioritize defending lane 0 (BundleDecision) over upgrading
		Assert.IsNotNull(bestAction);
		Assert.IsInstanceOf<BundleDecision>(bestAction);
		var bundle = (BundleDecision)bestAction;
		Assert.AreEqual(0, bundle.lane);
	}

	[Test]
	public void Test_SpawnDecision_DoNothing_NotCostEffective() {
		AIManager.Ins.Initialize();
		AIManager.Ins.costPenaltyFactor = 1000f; // heavy cost penalty
		BattleInfo.teamDict[Team.Right].resource = 10;

		// Hand has a card
		var card = MockBattleCard.Create(5, mockEntitySO);
		BattleInfo.teamDict[Team.Right].cards.Add(card);

		// No threats on grid
		var method = typeof(AIManager).GetMethod("FindBestAction", BindingFlags.NonPublic | BindingFlags.Instance);
		AIAction bestAction = (AIAction)method.Invoke(AIManager.Ins, null);

		// Since cost penalty is extremely high, spawning the card is not cost-effective. AI should do nothing.
		Assert.IsNull(bestAction);
	}

	[Test]
	public void Test_SpawnDecision_CheaperOptionPreferred() {
		AIManager.Ins.Initialize();
		AIManager.Ins.costPenaltyFactor = 200f;
		BattleInfo.teamDict[Team.Right].resource = 10;

		typeof(AIManager)
			.GetField("firstUpgrade", BindingFlags.NonPublic | BindingFlags.Instance)
			.SetValue(AIManager.Ins, false);

		// Add a weak threat on lane 0 so that one card is enough
		EntitySO weakSO = CreateTestEntitySO(5, 1);
		Entity attacker = new Entity(weakSO, Team.Left, 2f, 0, 1, true);
		entities[0].Add(attacker);

		// Two card options on hand:
		// Card A: cheap (cost 2)
		// Card B: expensive (cost 5)
		var cheapCard = MockBattleCard.Create(2, mockEntitySO);
		var expensiveCard = MockBattleCard.Create(5, mockEntitySO);
		BattleInfo.teamDict[Team.Right].cards.Add(expensiveCard);
		BattleInfo.teamDict[Team.Right].cards.Add(cheapCard);

		var method = typeof(AIManager).GetMethod("FindBestAction", BindingFlags.NonPublic | BindingFlags.Instance);
		AIAction bestAction = (AIAction)method.Invoke(AIManager.Ins, null);

		Assert.IsNotNull(bestAction);
		Assert.IsInstanceOf<BundleDecision>(bestAction);
		var bundle = (BundleDecision)bestAction;
		Assert.AreEqual(1, bundle.usedCards.Count);
		Assert.AreEqual(2, bundle.usedCards[0].cost, "AI should choose the cheaper card to defend");
	}
}
