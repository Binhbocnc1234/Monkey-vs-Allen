---
type: technical
module: ai-system
version: 1.0
audience: developer
related: [architecture-as-is, design-principles, execution-paths]
---

# AI System Overview

The AI system lives under `Assets/Scripts/Battlefield/AI/` in the `MvA.AI` assembly. It evaluates battle states and recommends actions by simulating outcomes using a lightweight model (`FakeEntity`) rather than full Unity objects.

# Goals

- Provide fast, deterministic evaluation for AI decisions.
- Avoid Unity runtime overhead during prediction (no `MonoBehaviour`, no `Transform`).
- Keep simulation generic and independent of concrete entity types.

# Core Components

## Simulator.cs

`Simulator` is a static, pure C# simulation engine. It converts real entities into `FakeEntity` snapshots and runs a simplified battle model.

**Key ideas:**
- `FakeEntity` is a POCO: `team`, `x`, `moveSpeed`, `range`, `danger`, `survivability`.
- Conversion from `IEntity` is generic; no concrete-entity branching.
- Simulation steps in fixed time slices (`SimulationStep = 1f`).

**Important methods:**
- `EvaluateBundle(...)` — public entry point. Converts inputs and evaluates score.
- `EvaluateBundle_2(...)` — runs two-phase sim (pre-spawn + post-spawn).
- `GetWorldSnapshot()` — returns simulated state for debugging/tests.

## AIManager*

AI behavior is split into multiple files for responsibilities:

- `AIManager(Editor).cs` — Editor tooling / debug helpers.
- `AIManager(Forecast).cs` — prediction logic and forecast evaluation.
- `AIManager(Upgrade).cs` — upgrade-related decision logic.
- `AIManager(subclassess).cs` — AI behavior variants (strategy overrides).

## EnemyManager.cs

High-level orchestration for AI-controlled enemies. Coordinates with the simulator to select actions and spawning decisions.

# Data Flow

```
Current Battle State (IEntity[] + IBattleCard[])
    ↓ convert to FakeEntity snapshots
Simulator.EvaluateBundle(...)
    ↓ returns numeric score
AI Manager chooses best action based on score
```

# Unity Integration

- Domain objects (entities, cards) are Unity `MonoBehaviour` types, but the simulator is pure C#.
- AI reads entity data through `IEntity` contract (`GetAssessPoint`, `GetRealMoveSpeed`, `ST.Range`, `gridPos.x`).
- No Unity rendering or UI access from AI or simulator.

# Constraints

- **No UI/TMP dependencies** inside `MvA.AI` (Phase 1 rule).
- Conversion must remain **generic**. If a change requires per-entity mapping logic, do not add it — refactor the contract instead.

# Testing

- Unit tests in `Assets/Tests/Editor/SimulatorTests.cs`.
- Tests log snapshots and assert pass (debug-focused tests).

# Extension Guidelines

- Add new simulation fields only if strictly needed by AI decisions.
- Prefer adding new values to `IEntity` contract or shared stats (e.g., `ST` enum) rather than adding concrete-type checks.
- Keep `FakeEntity` minimal — avoid simulating visuals or exact animations.
