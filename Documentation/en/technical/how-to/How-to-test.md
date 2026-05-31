---
type: technical
module: testing
version: 1.0
audience: developer, agent
related:
  - architecture-to-be
  - design-principles
  - software-development-process
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

## Testing Pyramid (Three Tiers)

```
        ╱╲
       ╱  ╲          Tier 3: Unity PlayMode
      ╱    ╲         Full simulation, Update(), physics,
     ╱      ╲       MonoBehaviours. ~30–60s per run.
    ╱────────╲
   ╱          ╲      Tier 2: Unity EditMode
  ╱            ╲    Pure C# inside Unity Editor.
 ╱──────────────╲  NO PlayMode overhead. ~5–15s per run.
╱                  ╲
╱  Tier 1: dotnet   ╲  Compile-only. C# syntax, types, references.
╱    build (~15s)    ╲  No Unity runtime. <20s.
╱────────────────────╲
```

### Tier 1 — Compile Check (`dotnet build`)

**What it does:** Verifies C# compilation — syntax, type safety, reference resolution — without launching Unity at all.

**How to run:**

```powershell
# Check core entity system (fastest, fewest dependencies)
dotnet build MvA.Core.csproj

# Check entity implementations
dotnet build MvA.Entities.csproj

# Check effect subclasses
dotnet build MvA.Effect.csproj

# Check framework
dotnet build MvA.Framework.Core.csproj

# Check multiple assemblies in parallel (each in a separate terminal)
```

**Why this works:** Unity generates standard `.csproj` files that reference Unity DLLs from the Editor installation. The .NET SDK (`dotnet`) resolves these references just like Visual Studio does, but from the command line. Since our architecture separates pure C# logic (`IEntity`, `IBehaviour`, `IUpdatePerFrame`) from Unity `MonoBehaviour` binding, most of our code lives in assemblies like `MvA.Core.csproj` that have few Unity dependencies.

**When to use:**
- After every code change (habit, like saving a file)
- Before committing
- Before pushing to remote
- In CI as a pre-check (fails fast without launching Unity)

**When NOT to use:**
- When the test involves Unity runtime features (Physics, SceneManager, Coroutines, Asset loading)
- When you need to verify game logic behavior, not just types

---

### Tier 2 — Unity EditMode Tests

**What they do:** NUnit tests that run inside the Unity Editor but **without** entering PlayMode. They can create `GameObject`s, call `new` on any class, and use some Unity APIs (math, logging, serialization). They cannot rely on `Update()`, `FixedUpdate`, frame lifecycle, or Physics simulation.

**How to run:**

1. Open the Unity Editor.
2. Open **Window > General > Test Runner**.
3. Select the **EditMode** tab.
4. Click **Run All** (or select specific tests).

Or via CLI (faster for automation):

```powershell
# Requires Unity CLI path (Unity 6000.3.10f1)
& "C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe" `
  -projectPath "." `
  -runTests `
  -testPlatform "EditMode" `
  -logFile "Logs/editmode-test.log"
```

**Why this tier exists:** EditMode tests skip the PlayMode overhead — no domain reload, no scene setup, no physics world. They are the **fastest Unity-aware tests** (~5–15s total). They catch logical bugs before you spend 30s entering PlayMode.

**What to put in EditMode tests:**
- Pure logic tests: entity factory, effect application, damage calculation, upgrade resolution
- Serialization tests: ScriptableObject values, EntitySO data
- Stateless system tests: `Simulator.EvaluateBundle`, `EffectSystem.ApplyEffect`
- Tests that only need `GameObject` + `Transform` (no lifecycle)

**Existing tests:**

`Assets/Tests/Editor/SimulatorTests.cs` — currently outdated (references old `Simulator` API). Preserved as spec reference. When `Simulator` is refactored for the new architecture (using `IEntity` instead of `FakeEntity`), these tests will be rewritten and re-enabled.

**New tests (Post-Refactor Phase 5):**

- `EntityTests.cs` — Entity instantiation, state management
- `BehaviourTests.cs` — Behaviour creation and execution
- `EffectControllerTests.cs` — Effect application and ticking
- `EContainerTests.cs` — Entity container lifecycle

**When to use:**
- When the test doesn't need `Update()` or `FixedUpdate`
- When you want to debug with Unity's Inspector or logging
- When the test creates ScriptableObjects or Assets

---

### Tier 3 — Unity PlayMode Tests

**What they do:** Full test environment — enters PlayMode, simulates frames, runs `Update()`, `FixedUpdate`, `Start()`, Physics, and coroutines. The slowest but most realistic tier.

**How to run:**

1. Open the Unity Editor.
2. Open **Window > General > Test Runner**.
3. Select the **PlayMode** tab.
4. Click **Run All**.

Or via CLI:

```powershell
# Requires Unity CLI path (Unity 6000.3.10f1)
& "C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe" `
  -projectPath "." `
  -runTests `
  -testPlatform "PlayMode" `
  -logFile "Logs/playmode-test.log"
```

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

---

## Why Our Architecture Makes Testing Easier

The architecture was designed with testability in mind. Here is how each design decision helps:

### Separation of Interface and Implementation

```
Pure C# (testable without Unity)     Unity Binding (requires PlayMode)
┌─────────────────────────────┐     ┌──────────────────────┐
│ IEntity       ←  Entity     │     │ EntityWrapper        │
│ IBehaviour    ←  Behaviour  │     │ BehaviourWrapper     │
│ IEffect                  │     │                      │
│ EffectController          │     │                      │
│ IUpdatePerFrame          │     │                      │
│ Timer                     │     │                      │
└─────────────────────────────┘     └──────────────────────┘
```

**Why this helps:** `IEntity`, `IBehaviour`, `EffectController`, `Timer` — all pure C#. They have NO Unity dependency. You can instantiate them in a test with `new`, set up state, call methods, and assert results. No `GameObject` required.

**This means Tier 1 (`dotnet build`) covers the core logic.** If the interface layer compiles, the logic layer compiles. The Unity-binding layer (`EntityWrapper`, `BehaviourWrapper`) is thin — mostly delegation.

### EntitySO as Data

`EntitySO` is a `ScriptableObject` that only holds data (stats, behaviour templates). It has no runtime methods. Testing an `Entity` constructed from SO data is just testing a data transformation:

```csharp
// EditMode test: no PlayMode needed
var so = ScriptableObject.CreateInstance<EntitySO>();
so.baseStats = new EntityStats { hp = 100, damage = 20 };
so.behaviourTemplates = new List<BehaviourTemplate> { /* ... */ };

var entity = Entity.CreateFromSO(so);  // pure C# factory
Assert.AreEqual(100, entity.HP);
```

### EffectController as Pure Logic

`EffectController` manages a list of active `IEffect`s, ticking them per-frame. It implements `IUpdatePerFrame` but the tick logic is pure C# — just loop through effects, call `Update()`, remove expired ones.

```csharp
// EditMode test
var controller = new EffectController();
controller.AddEffect(new Poisoning(5, 3)); // 5 damage, 3 ticks
controller.Update(); // tick
Assert.AreEqual(5, entity.CurrentHP); // HP reduced by 5
```

---

## Recommended Workflow

### Before every commit

```powershell
# 1. Compile check (Tier 1)
dotnet build MvA.Core.csproj
if ($LASTEXITCODE -ne 0) { Write-Output "Fix compilation errors"; exit }

# 2. Run EditMode tests (Tier 2)
# Open Unity Editor → Test Runner → EditMode → Run All
```

### Before merging to main

```powershell
# 1. Tier 1 + Tier 2 (as above)
# 2. Run PlayMode tests (Tier 3)
# Unity Editor → Test Runner → PlayMode → Run All
```

### During feature development

```
Write code                    → saves every few seconds
Run dotnet build (Tier 1)     → <20s, catch type errors
Write/run EditMode test       → ~15s, verify logic
Write/run PlayMode test       → ~60s, verify runtime
```

The cycle is: **code → `dotnet build` → EditMode test → PlayMode test**.

You should rarely need to enter PlayMode manually — PlayMode tests serve that role automatically.

---

## Test File Organization

All tests live in:

```
Assets/Tests/
├── Editor/          # EditMode tests (Tier 2)
│   ├── Core/        # IEntity, IBehaviour, EffectController
│   ├── Systems/     # Timer, UpdateManager, Simulator
│   └── Data/        # EntitySO, serialization
└── Runtime/         # PlayMode tests (Tier 3)  (future)
    ├── Entity/      # EntityWrapper, prefab lifecycle
    ├── Effects/     # Poisoning, OnFire runtime behavior
    └── Integration/ # Full scene tests
```

---

## Frequently Asked Questions

### Why not just use `dotnet test`?

NUnit tests that reference Unity types cannot run outside the Unity Editor because Unity's test runner (`UnityEngine.TestRunner`) provides special setup (domain reload, scene management). We could write tests in a separate non-Unity project, but those tests cannot touch any Unity API. Tier 1 (`dotnet build`) already covers pure C# compilation. Tier 2+ need the Unity Editor.

### Can we run tests from VS Code?

Yes — but only `dotnet build` (Tier 1) directly. For EditMode and PlayMode, you must switch to the Unity Editor window. VS Code's C# extension provides real-time diagnostics (<5s) which is essentially Tier 1 on every keystroke.

### Why are the old SimulatorTests commented out?

They reference a `Simulator` API that no longer exists (`EvaluateBundle_2`, `FakeEntity`, `GetWorldSnapshot`). The old code is preserved as a spec reference. When `Simulator` is refactored for the new architecture (using `IEntity` instead of `FakeEntity`), these tests will be rewritten and re-enabled.

### Test Assembly Definition (.asmdef)

`Assets/Tests/Editor/Tests.asmdef` must reference all game code assemblies:

```json
{
  "name": "Tests",
  "references": [
    "NUnit.Framework",
    "UnityEngine.TestRunner",
    "MvA.Core",
    "MvA.Entities",
    "MvA.Effect"
  ],
  "defineConstraints": [ "UNITY_INCLUDE_TESTS" ]
}
```

If tests fail to compile with "undefined reference to `MvA.Entities`", verify this .asmdef file is correct.

### How to run tests manually (for development)

1. Open the **Unity Editor**.
2. Open **Window > General > Test Runner**.
3. Select the **EditMode** tab.
4. If you don't see any tests, click **Run All** once to force a refresh (this triggers a compilation of the test assembly).
5. Once they appear, click **Run All** (or select specific test classes).

### Troubleshooting Test Compilation Errors
If you see "Scripts have compiler errors" or "Assembly has duplicate references":
- Ensure `Assets/Tests/Editor/Tests.asmdef` does not explicitly reference `UnityEngine.TestRunner` *and* have `TestAssemblies` enabled in `optionalUnityReferences`.
- If you see compiler errors in Unity, try:
    1. **Delete `Library/ScriptAssemblies` folder** (re-forces assembly rebuild).
    2. **Reimport All** (`Assets > Reimport All`).
    3. Ensure all game assemblies (`MvA.Core`, `MvA.Entities`, `MvA.Effect`) are correctly referenced in `Tests.asmdef`.

---

### What about CI?

For CI/CD:
- **Pull request pre-check:** `dotnet build MvA.Core.csproj && dotnet build MvA.Entities.csproj` — runs on any build agent with .NET SDK, no Unity required. Fails fast (<30s).
- **Full test gate:** Unity CLI with `-runTests -testPlatform "EditMode"` — requires a Unity license on the CI agent but runs in ~2 minutes.
- **Release gate:** Unity CLI with `-runTests -testPlatform "PlayMode"` — slowest but catches runtime bugs.
