---
type: technical
module: testing

audience: [developer, agent]
status: active
language: en
description: Canonical testing strategy for developers and agents; defines the tiered testing model, when to use each tier, and how the project should be validated.
related:
  - architecture-to-be
  - design-principles
  - software-development-process
  - agent-testing-playbook
  - how-to-run-test-manually
---

# How to Test

This guide explains the testing approach for this project: what to test, how to run tests, and **why** each method exists. The goal is to make testing fast enough that you actually do it, and reliable enough that you trust it.

---

## The Problem: Unity Testing Is Slow

Unity is not designed for test-driven development. Typical pain points:

| Pain point | Why it hurts |
|---|---|
| Unity Editor startup | 1–2 minutes before you can run anything |
| PlayMode test assembly reload | ~15–30s per PlayMode test run |
| Batch mode CLI | 2–3 minutes to build + run |
| MonoBehaviours in tests | Need full scene, GameObjects, frame lifecycle |

**If testing takes 2 minutes, you won't run it. If you don't run it, you ship bugs.**

The solution: a **three-tier testing pyramid** that matches cost to need.

---

## Three Tiers

The project uses three tiers:
- Tier 1: `dotnet build` for compile-only checks.
- Tier 2: Unity EditMode tests for fast Unity-aware logic.
- Tier 3: Unity PlayMode tests for full runtime simulation.

### Tier 1 — Compile Check (`dotnet build`)

**What it does:** Verifies C# compilation — syntax, type safety, reference resolution — without launching Unity at all.

**Why this works:** Unity generates standard `.csproj` files that reference Unity DLLs from the Editor installation. The .NET SDK (`dotnet`) resolves these references just like Visual Studio does, but from the command line. Since our architecture separates pure C# logic (`IEntity`, `IBehaviour`, `IUpdatePerFrame`) from Unity `MonoBehaviour` binding, most of our code lives in assemblies like `MvA.Core.csproj` that have few Unity dependencies.

**When to use:**
- After every code change (habit, like saving a file)
- Before committing
- Before pushing to remote
- In CI as a pre-check (fails fast without launching Unity)

**When not to use:**
- When the check depends on Unity runtime features such as Physics, SceneManager, Coroutines, or asset loading
- When you need runtime validation instead of compile validation

---

### Tier 2 — Unity EditMode Tests

**What they do:** NUnit tests that run inside the Unity Editor but **without** entering PlayMode. They can create `GameObject`s, call `new` on any class, and use some Unity APIs (math, logging, serialization). They cannot rely on `Update()`, `FixedUpdate`, frame lifecycle, or Physics simulation.

**Why this tier exists:** EditMode tests skip the PlayMode overhead — no domain reload, no scene setup, no physics world. They are the **fastest Unity-aware tests** (~5–15s total). They catch logical bugs before you spend 30s entering PlayMode.

**What to put in EditMode tests:**
- Pure logic tests: entity factory, effect application, damage calculation, upgrade resolution
- Serialization tests: ScriptableObject values, EntitySO data
- Stateless system tests: `Simulator.EvaluateBundle`, `EffectSystem.ApplyEffect`
- Tests that only need `GameObject` + `Transform` (no lifecycle)

**When to use:**
- When the test doesn't need `Update()` or `FixedUpdate`
- When you want to debug with Unity's Inspector or logging
- When the test creates ScriptableObjects or Assets

**When not to use:**
- When the behavior depends on frame updates, coroutines, physics, or scene lifecycle
- When the test needs full runtime simulation

---

### Tier 3 — Unity PlayMode Tests

**What they do:** Full test environment — enters PlayMode, simulates frames, runs `Update()`, `FixedUpdate`, `Start()`, Physics, and coroutines. The slowest but most realistic tier.

**Why this tier exists:** Some behaviors only manifest during runtime — entity movement interpolation, timed effect tick (Poisoning per-second with `IUpdatePerFrame`), animation state changes, prefab instantiation. PlayMode tests are the safety net for these.

**What to put in PlayMode tests:**
- Entity lifecycle: spawn → update → despawn
- Effect runtime: Poisoning dealing damage per-tick, OnFire applying visual
- Movement: entity walking along lane during frames
- Prefab binding: `EntityWrapper` ↔ `IEntity` synchronization
- Integration: full scene with multiple entities interacting

**When to use:**
- After EditMode tests pass (gate: EditMode first)
- When the feature involves `IUpdatePerFrame` or frame-dependent behavior
- When testing prefab instantiation or `EntityWrapper` lifecycle
- Before merging to main branch

**When not to use:**
- When the behavior can be covered by compile checks or EditMode tests
- When runtime simulation is unnecessary

---

## Why the Architecture Helps

The architecture was designed with testability in mind.

### Separation of Interface and Implementation

This project splits runtime code into two groups:
- Pure C# logic: `IEntity`, `IBehaviour`, `EffectController`, `Timer`, `IUpdatePerFrame`.
- Unity binding: `EntityWrapper`, `BehaviourWrapper`, and other components that need `GameObject`, scene, or PlayMode lifecycle.

Why this helps:
- Pure C# logic can be instantiated with `new`, exercised in EditMode, and validated without PlayMode.
- Unity binding stays thin and mostly delegates to the pure logic layer.
- Tier 1 (`dotnet build`) covers the core logic because the interface layer compiles independently.

### EntitySO as data

`EntitySO` is a `ScriptableObject` that holds data only, such as stats and behaviour templates. It has no runtime methods. That makes it a good fit for EditMode validation.

```csharp
// EditMode test: no PlayMode needed
var so = ScriptableObject.CreateInstance<EntitySO>();
so.baseStats = new EntityStats { hp = 100, damage = 20 };
so.behaviourTemplates = new List<BehaviourTemplate> { /* ... */ };

var entity = Entity.CreateFromSO(so);  // pure C# factory
Assert.AreEqual(100, entity.HP);
```

### EffectController as pure logic

`EffectController` manages active effects and applies per-frame ticking logic. The implementation is pure C#, so the core behavior can be tested without entering PlayMode.

## Reference files

If you need an execution-oriented companion for automation, see tools/run_tests.ps1