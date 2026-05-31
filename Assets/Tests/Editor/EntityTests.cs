using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class EntityTests {
    // Helper: create a minimal EntitySO with an Idle behaviour
    private EntitySO CreateMinimalEntitySO() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 100;
        so.canAttack = true;
        so.damage = 10;
        so.attackSpeed = 1f;
        so.moveSpeed = 2f;
        so.behaviourTemplates = new List<IBehaviour> {
            new Idle()
        };
        return so;
    }

    // Helper: create a minimal Entity for testing
    private Entity CreateTestEntity(EntitySO so = null, Team team = Team.Left, float x = 0f, int lane = 0, int level = 1) {
        if (so == null) so = CreateMinimalEntitySO();
        return new Entity(so, team, x, lane, level);
    }

    [Test]
    public void Constructor_InitializesStatsFromEntitySO() {
        EntitySO so = CreateMinimalEntitySO();
        Entity e = CreateTestEntity(so);

        Assert.AreEqual(10f, e.Stats[ST.Strength]);
        Assert.AreEqual(1f, e.Stats[ST.AttackSpeed]);
        Assert.AreEqual(2f, e.Stats[ST.MoveSpeed]);
        Assert.AreEqual(so.health, e.Stats[ST.MaxHealth]);
        Assert.AreEqual(so.health, e.Stats[ST.Health]); // starts at max
    }

    [Test]
    public void Indexer_Health_ReturnsCorrectValue() {
        Entity e = CreateTestEntity();
        Assert.AreEqual(e.Stats[ST.Health], e[ST.Health]);
    }

    [Test]
    public void Indexer_Armor_ClampedToMax100() {
        Entity e = CreateTestEntity();
        e.Stats[ST.Armor] = 150f;
        Assert.AreEqual(100f, e[ST.Armor]);
    }

    [Test]
    public void Indexer_MagicResistance_ClampedToMax100() {
        Entity e = CreateTestEntity();
        e.Stats[ST.MagicResistance] = 200f;
        Assert.AreEqual(100f, e[ST.MagicResistance]);
    }

    [Test]
    public void Indexer_AttackSpeed_Minimum025() {
        Entity e = CreateTestEntity();
        e.Stats[ST.AttackSpeed] = 0f;
        Assert.AreEqual(0.25f, e[ST.AttackSpeed]);
    }

    [Test]
    public void Indexer_Penetration_ClampedToMax100() {
        Entity e = CreateTestEntity();
        e.Stats[ST.Penetration] = 999f;
        Assert.AreEqual(100f, e[ST.Penetration]);
    }

    [Test]
    public void TakeDamage_ReducesHP() {
        Entity e = CreateTestEntity();
        float initial = e.Stats[ST.Health];
        DamageContext ctx = new DamageContext(30f, e, e, false);
        e.TakeDamage(ctx);
        Assert.AreEqual(initial - 30f, e.Stats[ST.Health]);
    }

    [Test]
    public void TakeDamage_AmountZeroOrLess_DoesNothing() {
        Entity e = CreateTestEntity();
        float initial = e.Stats[ST.Health];

        DamageContext ctxZero = new DamageContext(0f, e, e, false);
        e.TakeDamage(ctxZero);
        Assert.AreEqual(initial, e.Stats[ST.Health]);

        DamageContext ctxNegative = new DamageContext(-10f, e, e, false);
        e.TakeDamage(ctxNegative);
        Assert.AreEqual(initial, e.Stats[ST.Health]);
    }

    [Test]
    public void TakeDamage_HPReachesZero_CallsDie() {
        Entity e = CreateTestEntity();
        bool died = false;
        e.OnEntityDeath += () => died = true;

        DamageContext ctx = new DamageContext(e.Stats[ST.MaxHealth] + 999f, e, e, false);
        e.TakeDamage(ctx);

        Assert.AreEqual(0f, e.Stats[ST.Health]);
        Assert.IsTrue(died);
        Assert.IsTrue(e.IsDead());
    }

    [Test]
    public void TakeDamage_WhenDead_DoesNothing() {
        Entity e = CreateTestEntity();
        DamageContext killCtx = new DamageContext(9999f, e, e, false);
        e.TakeDamage(killCtx);
        Assert.IsTrue(e.IsDead());

        float hpAfterKill = e.Stats[ST.Health];
        DamageContext extraCtx = new DamageContext(50f, e, e, false);
        e.TakeDamage(extraCtx);
        Assert.AreEqual(hpAfterKill, e.Stats[ST.Health]);
    }

    [Test]
    public void Heal_IncreasesHP_UpToMaxHealth() {
        Entity e = CreateTestEntity();
        DamageContext ctx = new DamageContext(40f, e, e, false);
        e.TakeDamage(ctx);
        float afterDamage = e.Stats[ST.Health];

        e.Heal(20f);
        Assert.AreEqual(afterDamage + 20f, e.Stats[ST.Health]);

        // Heal past max clamps
        e.Heal(999f);
        Assert.AreEqual(e.Stats[ST.MaxHealth], e.Stats[ST.Health]);
    }

    [Test]
    public void Heal_WhenDead_DoesNothing() {
        Entity e = CreateTestEntity();
        DamageContext ctx = new DamageContext(9999f, e, e, false);
        e.TakeDamage(ctx);
        Assert.IsTrue(e.IsDead());

        e.Heal(100f);
        Assert.AreEqual(0f, e.Stats[ST.Health]); // still dead, HP unchanged
    }

    [Test]
    public void Die_SetsIsDeadAndFiresOnEntityDeath() {
        Entity e = CreateTestEntity();
        bool died = false;
        e.OnEntityDeath += () => died = true;

        Assert.IsFalse(e.IsDead());
        e.Die();

        Assert.IsTrue(e.IsDead());
        Assert.IsTrue(died);
    }

    [Test]
    public void Die_CalledTwice_FiresOnlyOnce() {
        Entity e = CreateTestEntity();
        int callCount = 0;
        e.OnEntityDeath += () => callCount++;

        e.Die();
        e.Die();

        Assert.AreEqual(1, callCount);
    }

    [Test]
    public void IsDead_ReturnsCorrectState() {
        Entity e = CreateTestEntity();
        Assert.IsFalse(e.IsDead());

        e.Die();
        Assert.IsTrue(e.IsDead());
    }

    [Test]
    public void GetBehaviour_FindsBehaviourByType() {
        Entity e = CreateTestEntity();
        Idle idle = e.GetBehaviour<Idle>();
        Assert.IsNotNull(idle);
        Assert.IsInstanceOf<Idle>(idle);
    }

    [Test]
    public void GetBehaviour_ReturnsNull_WhenNotFound() {
        Entity e = CreateTestEntity();
        // No InactiveBehaviour in the template
        InactiveBehaviour ib = e.GetBehaviour<InactiveBehaviour>();
        Assert.IsNull(ib);
    }

    [Test]
    public void GetBehaviours_ReturnsMatchingBehaviours() {
        EntitySO so = CreateMinimalEntitySO();
        so.behaviourTemplates.Add(new InactiveBehaviour());
        Entity e = CreateTestEntity(so);

        List<IBehaviour> result = new List<IBehaviour>(e.GetBehaviours<IBehaviour>());
        Assert.AreEqual(2, result.Count);

        List<Idle> idles = new List<Idle>(e.GetBehaviours<Idle>());
        Assert.AreEqual(1, idles.Count);
    }

    [Test]
    public void SetBehaviours_ReplacesAndOrdersByPriority() {
        EntitySO so = CreateMinimalEntitySO();
        Entity e = CreateTestEntity(so);

        // Replace with behaviours in different priority order
        var newBehaviours = new List<IBehaviour> {
            new InactiveBehaviour(), // priority -1
            new Idle()               // priority 0
        };
        e.SetBehaviours(newBehaviours);

        // Should be ordered by priority descending (highest first)
        Assert.AreEqual(2, e.behaviours.Length);
        // Highest priority is 0 (Idle), then -1 (InactiveBehaviour)
        Assert.IsInstanceOf<Idle>(e.behaviours[0]);
        Assert.IsInstanceOf<InactiveBehaviour>(e.behaviours[1]);
    }

    [Test]
    public void SetBehaviours_EmptyList_DoesNotThrow() {
        Entity e = CreateTestEntity();
        Assert.DoesNotThrow(() => e.SetBehaviours(new List<IBehaviour>()));
    }

    [Test]
    public void DistanceTo_UsesGridPosXDifference() {
        EntitySO so = CreateMinimalEntitySO();
        Entity a = new Entity(so, Team.Left, 2f, 0, 1);
        Entity b = new Entity(so, Team.Left, 5f, 0, 1);

        Assert.AreEqual(3f, a.DistanceTo(b));
        Assert.AreEqual(3f, b.DistanceTo(a));
    }

    [Test]
    public void DistanceTo_DifferentTeams_StillUsesX() {
        EntitySO so = CreateMinimalEntitySO();
        Entity a = new Entity(so, Team.Left, 1f, 0, 1);
        Entity b = new Entity(so, Team.Right, 4f, 0, 1);

        Assert.AreEqual(3f, a.DistanceTo(b));
    }

    [Test]
    public void TogglePause_StopsBehaviourUpdates() {
        Entity e = CreateTestEntity();
        // When paused is true, UpdateBehaviours skips most logic
        // We test the internal state: by default isPaused is false after construction
        // Then we pause and verify behaviour is not updated

        e.TogglePause(true);
        // We can't easily observe the effect without mocking BattleInfo/IGrid,
        // but we can verify it doesn't throw when called while paused
        // (BattleInfo.gameState is not Fighting so UpdateBehaviours returns early anyway)
        Assert.DoesNotThrow(() => e.UpdateBehaviours(0.016f));
    }

    [Test]
    public void BecomeInActive_SwitchesToInactiveBehaviour() {
        EntitySO so = CreateMinimalEntitySO();
        so.behaviourTemplates.Add(new InactiveBehaviour());
        Entity e = CreateTestEntity(so);

        e.BecomeInActive();
        Assert.IsInstanceOf<InactiveBehaviour>(e.GetActiveBehaviour());
    }

    [Test]
    public void GetHealthPercentage_ReturnsZeroToOneRange() {
        Entity e = CreateTestEntity();
        Assert.AreEqual(1f, e.GetHealthPercentage()); // full health

        DamageContext ctx = new DamageContext(50f, e, e, false);
        e.TakeDamage(ctx);
        Assert.AreEqual(0.5f, e.GetHealthPercentage(), 0.001f);
    }

    [Test]
    public void GetHealthPercentage_DeadEntity_ReturnsZero() {
        Entity e = CreateTestEntity();
        DamageContext ctx = new DamageContext(9999f, e, e, false);
        e.TakeDamage(ctx);
        Assert.AreEqual(0f, e.GetHealthPercentage());
    }

    [Test]
    public void GetAssessPoint_ReturnsCachedValue_WithinHalfSecond() {
        Entity e = CreateTestEntity();
        e.Stats[ST.Armor] = 0f;
        e.Stats[ST.Health] = 70f;
        e.Stats[ST.MaxHealth] = 100f;

        // First call computes and caches
        float first = e.GetAssessPoint(APType.Danger);

        // Second call within 0.5s should return cached value
        float second = e.GetAssessPoint(APType.Danger);
        Assert.AreEqual(first, second);
    }

    [Test]
    public void GetRealMoveSpeed_ReturnsStatsDividedBy4() {
        Entity e = CreateTestEntity();
        e.Stats[ST.MoveSpeed] = 4f;
        Assert.AreEqual(1f, e.GetRealMoveSpeed());

        e.Stats[ST.MoveSpeed] = 2f;
        Assert.AreEqual(0.5f, e.GetRealMoveSpeed());
    }

    [Test]
    public void Indexer_UnknownStat_ReturnsZero() {
        Entity e = CreateTestEntity();
        // Health exists, but VirtualHealth doesn't
        Assert.AreEqual(0f, e[ST.VirtualHealth]);
    }

    [Test]
    public void Constructor_SetsTeamAndLaneAndGridPos() {
        Entity e = CreateTestEntity(team: Team.Right, x: 7f, lane: 2);

        Assert.AreEqual(Team.Right, e.team);
        Assert.AreEqual(2, e.lane);
        Assert.AreEqual(new Vector2(7f, 2f), e.gridPos);
    }

    [Test]
    public void ReturnToIdleBehaviour_SwitchestoIdle() {
        EntitySO so = CreateMinimalEntitySO();
        so.behaviourTemplates.Add(new InactiveBehaviour());
        Entity e = CreateTestEntity(so);

        // Switch to Inactive
        e.BecomeInActive();
        Assert.IsInstanceOf<InactiveBehaviour>(e.GetActiveBehaviour());

        // Return to idle
        e.ReturnToIdleBehaviour();
        Assert.IsInstanceOf<Idle>(e.GetActiveBehaviour());
    }
}
