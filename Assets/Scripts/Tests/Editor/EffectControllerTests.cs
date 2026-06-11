using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class EffectControllerTests {
    // A concrete Effect subclass for testing
    private class TestEffect : Effect {
        public TestEffect(float duration = -1, int strength = 1) : base(duration, strength) { }
        public override int GetDangerPoint() => 0;
    }

    // A stackable Effect for testing
    private class TestStackableEffect : Effect, IStackable {
        public int stackCount = 0;
        public TestStackableEffect(float duration = -1, int strength = 1) : base(duration, strength) { }
        public void Stack(int amount) { stackCount += amount; }
        public override int GetDangerPoint() => 0;
    }

    // A stat-modifying effect for testing
    private class TestStatEffect : Effect, IModifyStat {
        private readonly ST statType;
        private readonly float value;
        private readonly Operator op;
        public TestStatEffect(ST statType, float value, Operator op = Operator.Addition) : base(-1) {
            this.statType = statType;
            this.value = value;
            this.op = op;
        }
        public List<StatModifier> ModifyStat() {
            return new List<StatModifier> { new StatModifier(op, statType, value) };
        }
        public override int GetDangerPoint() => 0;
        public override bool IsIdentical(Effect effect) {
            if (effect is TestStatEffect other) {
                return this.statType == other.statType && this.op == other.op && this.value == other.value;
            }
            return false;
        }
    }

    // A damage output modifier effect
    private class TestDamageOutputEffect : Effect, IDamageOutputModifier {
        public float modifierAmount;
        public TestDamageOutputEffect(float modifierAmount) : base(-1) {
            this.modifierAmount = modifierAmount;
            isDebuff = false;
        }
        public void ModifyDamage(DamageContext ctx) {
            ctx.AddModifier(new DamageModifier(Operator.Addition, modifierAmount));
        }
        public override int GetDangerPoint() => 0;
    }

    private Entity CreateMinimalEntity() {
        EntitySO so = ScriptableObject.CreateInstance<EntitySO>();
        so.id = Guid.NewGuid().ToString();
        so.health = 100;
        so.behaviourTemplates = new List<IBehaviour> { new Idle() };
        return new Entity(so, Team.Left, 0f, 0, 1);
    }

    [Test]
    public void Constructor_InitializesEmpty() {
        Entity e = CreateMinimalEntity();
        EffectController ec = (EffectController)e.GetEffectable();
        Assert.IsNotNull(ec);
    }

    [Test]
    public void AddEffect_AddsEffect() {
        Entity e = CreateMinimalEntity();
        EffectController ec = (EffectController)e.GetEffectable();

        TestEffect effect = new TestEffect();
        ec.ApplyEffect(effect);
        ec.Flush();

        Assert.IsTrue(ec.HaveEffect(typeof(TestEffect)));
    }

    [Test]
    public void ApplyEffect_IdenticalEffect_ResetsDuration() {
        Entity e = CreateMinimalEntity();
        EffectController ec = (EffectController)e.GetEffectable();

        var effect = new TestEffect(duration: 5f);
        ec.ApplyEffect(effect);
        ec.Flush();
        Assert.IsTrue(effect.HaveDuration);

        float originalRemaining = effect.lifeTimer.remainingTime;

        // Simulate some time passing
        effect.lifeTimer.SetCurTime(2f);

        // Apply identical effect - should reset duration
        var effect2 = new TestEffect(duration: 5f);
        ec.ApplyEffect(effect2);
        ec.Flush();

        // The original effect should have been reset to totalTime
        Assert.AreEqual(5f, effect.lifeTimer.remainingTime, 0.001f);
    }

    [Test]
    public void ApplyEffect_StackableEffect_CallsStack() {
        Entity e = CreateMinimalEntity();
        EffectController ec = (EffectController)e.GetEffectable();

        var effect = new TestStackableEffect(strength: 1);
        ec.ApplyEffect(effect);
        ec.Flush();

        var effect2 = new TestStackableEffect(strength: 2);
        ec.ApplyEffect(effect2);
        ec.Flush();

        Assert.AreEqual(2, effect.stackCount);
    }

    [Test]
    public void HaveEffect_ReturnsTrue_WhenEffectExists() {
        Entity e = CreateMinimalEntity();
        EffectController ec = (EffectController)e.GetEffectable();

        ec.ApplyEffect(new TestEffect());
        ec.Flush();
        Assert.IsTrue(ec.HaveEffect(typeof(TestEffect)));
    }

    [Test]
    public void HaveEffect_ReturnsFalse_WhenEffectDoesNotExist() {
        Entity e = CreateMinimalEntity();
        EffectController ec = (EffectController)e.GetEffectable();

        Assert.IsFalse(ec.HaveEffect(typeof(TestEffect)));
        Assert.IsFalse(ec.HaveEffect(typeof(TestStackableEffect)));
    }

    [Test]
    public void Update_TicksEffects_RemovesExpired() {
        Entity e = CreateMinimalEntity();
        EffectController ec = (EffectController)e.GetEffectable();

        var effect = new TestEffect(duration: 1f);
        ec.ApplyEffect(effect);
        ec.Flush();

        // Fails if GetFinalStat throws when effect has IModifyStat but effect list is empty after removal
        Assert.IsTrue(ec.HaveEffect(typeof(TestEffect)));

        // Tick past the duration
        ec.Update(2f);
        ec.Flush();

        // Effect should be removed
        Assert.IsFalse(ec.HaveEffect(typeof(TestEffect)));
    }

    [Test]
    public void Reset_ClearsAllEffects() {
        Entity e = CreateMinimalEntity();
        EffectController ec = (EffectController)e.GetEffectable();

        ec.ApplyEffect(new TestEffect());
        ec.ApplyEffect(new TestStackableEffect());
        ec.Flush();
        Assert.IsTrue(ec.HaveEffect(typeof(TestEffect)));

        ec.Reset();

        Assert.IsFalse(ec.HaveEffect(typeof(TestEffect)));
        Assert.IsFalse(ec.HaveEffect(typeof(TestStackableEffect)));
    }

    [Test]
    public void ProcessDamageOutput_CallsIDamageOutputModifier() {
        Entity entity = CreateMinimalEntity();
        Entity attacker = CreateMinimalEntity();
        EffectController ec = (EffectController)entity.GetEffectable();

        var outputEffect = new TestDamageOutputEffect(modifierAmount: 10f);
        ec.ApplyEffect(outputEffect);
        ec.Flush();

        DamageContext ctx = new DamageContext(50f, attacker, entity, false);
        ec.ProcessDamageOutput(ctx);

        // The modifier should have been applied (adds 10)
        Assert.AreEqual(60f, ctx.amount);
    }

    [Test]
    public void ProcessDamageInput_AppliesArmorModification() {
        Entity e = CreateMinimalEntity();
        e.Stats[ST.Armor] = 50f; // 50% physical damage reduction
        Entity attacker = CreateMinimalEntity();
        attacker.Stats[ST.Penetration] = 0f;

        EffectController ec = (EffectController)e.GetEffectable();
        DamageContext ctx = new DamageContext(100f, attacker, e, false);

        ec.ProcessDamageInput(ctx);

        // Armor 50 => (1 - 50/100) = 0.5 multiplier. 100 * 0.5 = 50
        Assert.AreEqual(50f, ctx.amount, 0.01f);
    }

    [Test]
    public void ProcessDamageInput_AppliesMagicResistanceForMagicalDamage() {
        Entity e = CreateMinimalEntity();
        e.Stats[ST.MagicResistance] = 25f; // 25% magic damage reduction
        Entity attacker = CreateMinimalEntity();
        attacker.Stats[ST.MagicPenetration] = 0f;

        EffectController ec = (EffectController)e.GetEffectable();
        DamageContext ctx = new DamageContext(100f, attacker, e, isMagicalDamage: true);

        ec.ProcessDamageInput(ctx);

        // MagicResistance 25 => (1 - 25/100) = 0.75 multiplier. 100 * 0.75 = 75
        Assert.AreEqual(75f, ctx.amount, 0.01f);
    }

    [Test]
    public void ProcessDamageInput_ArmorCannotGoBelowZero() {
        Entity e = CreateMinimalEntity();
        e.Stats[ST.Armor] = 0f;
        Entity attacker = CreateMinimalEntity();
        attacker.Stats[ST.Penetration] = 0f;

        EffectController ec = (EffectController)e.GetEffectable();
        DamageContext ctx = new DamageContext(100f, attacker, e, false);

        ec.ProcessDamageInput(ctx);

        // Armor 0 => multiplier 1.0
        Assert.AreEqual(100f, ctx.amount, 0.01f);
    }

    [Test]
    public void GetFinalStat_ReturnsModifiedStatValue() {
        Entity e = CreateMinimalEntity();
        e.Stats[ST.Strength] = 10f;
        EffectController ec = (EffectController)e.GetEffectable();

        // Add an effect that adds 5 to Strength
        ec.ApplyEffect(new TestStatEffect(ST.Strength, 5f, Operator.Addition));
        ec.Flush();

        float finalValue = ec.GetFinalStat(ST.Strength);
        Assert.AreEqual(15f, finalValue);
    }

    [Test]
    public void GetFinalStat_AppliesMultiplier() {
        Entity e = CreateMinimalEntity();
        e.Stats[ST.Strength] = 10f;
        EffectController ec = (EffectController)e.GetEffectable();

        // Add an effect that multiplies Strength by 2
        ec.ApplyEffect(new TestStatEffect(ST.Strength, 2f, Operator.Multiply));
        ec.Flush();

        float finalValue = ec.GetFinalStat(ST.Strength);
        Assert.AreEqual(20f, finalValue);
    }

    [Test]
    public void GetFinalStat_CombinesAdditionAndMultiplier() {
        Entity e = CreateMinimalEntity();
        e.Stats[ST.Strength] = 10f;
        EffectController ec = (EffectController)e.GetEffectable();

        ec.ApplyEffect(new TestStatEffect(ST.Strength, 5f, Operator.Addition));
        ec.ApplyEffect(new TestStatEffect(ST.Strength, 2f, Operator.Multiply));
        ec.Flush();

        float finalValue = ec.GetFinalStat(ST.Strength);
        // (10 + 5) * 2 = 30
        Assert.AreEqual(30f, finalValue);
    }
}
