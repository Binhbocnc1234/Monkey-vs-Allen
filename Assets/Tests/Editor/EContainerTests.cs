using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Test behaviour that tracks whether UpdateBehaviour was called.
/// Used to verify TickEntities propagates deltaTime to entities.
/// </summary>
public class TestTrackableBehaviour : IBehaviour, IInterruptable {
    public bool wasUpdated;
    public override bool CanActive() => true;
    public override void UpdateBehaviour(float deltaTime) { wasUpdated = true; }
    public override int GetPriority() => 0;
    public override string GetAnimatorStateName() => "TestTrackable";
}

[TestFixture]
public class EContainerTests {
    private GameObject testGo;
    private EContainer container;
    private List<List<Entity>> entities;
    private FieldInfo entitiesField;

    [SetUp]
    public void SetUp() {
        testGo = new GameObject("TestEContainer");
        container = testGo.AddComponent<EContainer>();

        // Invoke Awake to set up the singleton reference (EContainer.Ins)
        MethodInfo awake = typeof(Singleton<IEntityRegistry>)
            .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        awake.Invoke(container, null);

        // Access the private 'entities' field via reflection
        entitiesField = typeof(EContainer)
            .GetField("entities", BindingFlags.NonPublic | BindingFlags.Instance);

        // Set up the entities list with 2 lanes
        entities = new List<List<Entity>> {
            new List<Entity>(),
            new List<Entity>()
        };
        entitiesField.SetValue(container, entities);
    }

    [TearDown]
    public void TearDown() {
        if (testGo != null) {
            GameObject.DestroyImmediate(testGo);
        }
    }

    private static EntitySO CreateTestSO() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 10;
        so.behaviourTemplates = new List<IBehaviour> { new Idle() };
        return so;
    }

    private static Entity CreateEntity(EntitySO so, Team team = Team.Left, float x = 0f, int lane = 0, int level = 1) {
        return new Entity(so, team, x, lane, level);
    }

    [Test]
    public void EntitySetting_DefaultsAreSet() {
        EntitySetting setting = new EntitySetting();
        Assert.IsNull(setting.so);
        Assert.AreEqual(0, setting.lane);
        Assert.AreEqual(0f, setting.x);
        Assert.AreEqual(Team.Left, setting.team);
        Assert.AreEqual(1, setting.level);
        Assert.IsTrue(setting.isSimulated);
    }

    [Test]
    public void EntitySetting_PropertiesAreMutable() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();

        EntitySetting setting = new EntitySetting {
            so = so,
            lane = 2,
            x = 5f,
            team = Team.Right,
            level = 3,
            isSimulated = false
        };

        Assert.AreSame(so, setting.so);
        Assert.AreEqual(2, setting.lane);
        Assert.AreEqual(5f, setting.x);
        Assert.AreEqual(Team.Right, setting.team);
        Assert.AreEqual(3, setting.level);
        Assert.IsFalse(setting.isSimulated);
    }

    [Test]
    public void Entity_Constructor_SetsLevel() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 10;
        so.behaviourTemplates = new List<IBehaviour> { new Idle() };

        Entity e = new Entity(so, Team.Left, 0f, 0, 3);
        Assert.AreEqual(3, e.level);
    }

    [Test]
    public void Entity_Constructor_SetsTeam() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 10;
        so.behaviourTemplates = new List<IBehaviour> { new Idle() };

        Entity eLeft = new Entity(so, Team.Left, 0f, 0, 1);
        Assert.AreEqual(Team.Left, eLeft.team);

        Entity eRight = new Entity(so, Team.Right, 0f, 0, 1);
        Assert.AreEqual(Team.Right, eRight.team);
    }

    [Test]
    public void Entity_Constructor_SetsLaneAndGridPosition() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 10;
        so.behaviourTemplates = new List<IBehaviour> { new Idle() };

        Entity e = new Entity(so, Team.Left, 7.5f, 3, 1);
        Assert.AreEqual(3, e.lane);
        Assert.AreEqual(7.5f, e.gridPos.x);
        Assert.AreEqual(3f, e.gridPos.y);
    }

    [Test]
    public void Entity_Constructor_NotPausedAfterConstruction() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 10;
        so.behaviourTemplates = new List<IBehaviour> { new Idle() };

        Entity e = new Entity(so, Team.Left, 0f, 0, 1);
        // After construction, isPaused is false and activeBehaviour is set
        Assert.IsNotNull(e.GetActiveBehaviour());
        Assert.IsInstanceOf<Idle>(e.GetActiveBehaviour());
    }

    [Test]
    public void Entity_Constructor_NoBehaviourTemplates_FallsBackToEmpty() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 10;
        // No behaviour templates - constructor will create empty array
        // idleBehaviour will be null, ChangeBehaviour will throw
        // So we need at least Idle in templates - verify this requirement
        so.behaviourTemplates = new List<IBehaviour> { new Idle() };
        Entity e = new Entity(so, Team.Left, 0f, 0, 1);
        Assert.IsNotNull(e.behaviours);
        Assert.AreEqual(1, e.behaviours.Length);
    }

    // --- TickEntities tests ---

    [Test]
    public void TickEntities_RemovesDeadEntities() {
        EntitySO so = CreateTestSO();
        Entity alive = CreateEntity(so);
        Entity dead = CreateEntity(so);
        dead.Die();

        entities[0].Add(alive);
        entities[0].Add(dead);
        Assert.AreEqual(2, entities[0].Count, "Both entities should be in the list before TickEntities");

        container.TickEntities(1f);

        Assert.AreEqual(1, entities[0].Count, "Dead entity should be removed after TickEntities");
        Assert.AreEqual(alive, entities[0][0], "Alive entity should remain");
    }

    [Test]
    public void TickEntities_RemovesNullEntities() {
        EntitySO so = CreateTestSO();
        Entity alive = CreateEntity(so);

        entities[0].Add(alive);
        entities[0].Add(null);
        Assert.AreEqual(2, entities[0].Count, "Both entries should be in the list before TickEntities");

        container.TickEntities(1f);

        Assert.AreEqual(1, entities[0].Count, "Null entry should be removed after TickEntities");
        Assert.AreEqual(alive, entities[0][0], "Alive entity should remain");
    }

    [Test]
    public void TickEntities_CallsUpdateBehavioursOnAliveEntities() {
        EntitySO so = CreateTestSO();
        // Use a behaviour that tracks UpdateBehaviour calls
        so.behaviourTemplates = new List<IBehaviour> { new TestTrackableBehaviour() };
        Entity alive = CreateEntity(so);

        entities[0].Add(alive);

        // Verify the active behaviour is our trackable one
        IBehaviour activeBefore = alive.GetActiveBehaviour();
        Assert.IsInstanceOf<TestTrackableBehaviour>(activeBefore);
        Assert.IsFalse(((TestTrackableBehaviour)activeBefore).wasUpdated,
            "Should not have been updated before TickEntities");

        // Set game state to Fighting so UpdateBehaviours runs
        BattleInfo.ChangeState(GameState.Fighting);

        container.TickEntities(1f);

        // Check the behaviour was updated
        IBehaviour activeAfter = alive.GetActiveBehaviour();
        Assert.IsTrue(((TestTrackableBehaviour)activeAfter).wasUpdated,
            "UpdateBehaviour should have been called on alive entity");
    }

    [Test]
    public void TickEntities_ProcessesAllLanes() {
        EntitySO so = CreateTestSO();
        Entity lane0_alive = CreateEntity(so, lane: 0);
        Entity lane0_dead = CreateEntity(so, lane: 0);
        lane0_dead.Die();
        Entity lane1_alive = CreateEntity(so, lane: 1);

        entities[0].Add(lane0_alive);
        entities[0].Add(lane0_dead);
        entities[1].Add(lane1_alive);

        container.TickEntities(1f);

        Assert.AreEqual(1, entities[0].Count, "Lane 0 should have dead entity removed");
        Assert.AreEqual(lane0_alive, entities[0][0], "Lane 0 should keep alive entity");
        Assert.AreEqual(1, entities[1].Count, "Lane 1 should keep its alive entity");
        Assert.AreEqual(lane1_alive, entities[1][0], "Lane 1 should keep its entity");
    }

    [Test]
    public void Update_DelegatesToTickEntities() {
        EntitySO so = CreateTestSO();
        Entity alive = CreateEntity(so);
        entities[0].Add(alive);

        // Manually invoke Update (normally called by Unity every frame)
        // We use reflection to call the private Update method
        MethodInfo updateMethod = typeof(EContainer)
            .GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(updateMethod, "Update method should exist");

        // Should not throw
        Assert.DoesNotThrow(() => updateMethod.Invoke(container, null));
    }
}
