---
type: technical

audience: [developer]
status: draft
language: en
description: Draft implementation plan for simulator converter work.
related: []
---

# Simulator — generic converter design

## Goal

Convert IEntity/IBattleCard → FakeEntity for AI simulation, using **zero concrete-type checks**. One converter, all entities.

## Current state (working, already generic)

Simulator.cs has 2 converters (37 lines total):

```
Entity→FakeEntity  (line 28-51): reads IEntity interface only
Card→FakeEntity    (line 53-82): reads IBattleCard → IEntity prefab
```

Both are already type-agnostic. They read:
- `entity.team`, `entity.gridPos.x`
- `entity.GetRealMoveSpeed()` — abstract method on IEntity
- `entity[ST.Range]` — indexer on IEntity
- `entity.GetAssessPoint(APType.Danger|Defend)` — abstract method on IEntity

**No concrete type check anywhere.** This is correct by design.

## The contract these converters rely on

```
IEntity (abstract MonoBehaviour)
  ├── team          → Team
  ├── gridPos       → Vector2
  ├── GetRealMoveSpeed()          → float
  ├── this[ST stat]               → float  (indexer into Stats dictionary)
  └── GetAssessPoint(APType)      → float
```

The converter reads only from this contract. Adding a new concrete entity (BasicMonkey, Slimz, whatever) **does not** change the converter.

## Why the current design is correct

| Concern | Status |
|---------|--------|
| Converter lines depend on entity count? | No — 37 lines, handles all entities |
| New entity needs converter changes? | No — implements IEntity, conversion auto-works |
| Simulation needs MonoBehaviour? | No — FakeEntity is POCO |

## Two options going forward

### Option A — Keep FakeEntity (current approach)

Keep `Simulator.cs` pure C# with `FakeEntity` POCO. The 37-line converter stays as-is.

- Pros: Zero refactoring. Simulator stays decoupled from Unity.
- Cons: Duplication (FakeEntity fields mirror IEntity stats). Need to keep converter in sync if IEntity contract changes.

### Option B — Simulate directly on IEntity (no FakeEntity)

Make the Simulator accept `IEntity` directly and read stats from them. But this means the Simulator depends on `MonoBehaviour`-derived types, losing the "pure C# simulation" property.

- Pros: Eliminates converter entirely. No duplication.
- Cons: Simulator can't run outside Unity (needs MonoBehaviour). Unit tests require scene/Instantiate.

**Recommendation: Keep Option A.** The converter is small, generic, and already correct. The duplication is negligible (5 fields vs a complex entity tree). Option B would lose the ability to run AI prediction off the main thread or headless.

## The ConvertToFake extension (optional cleanup)

If you want to formalize the converter, a single extension method:

```csharp
public static FakeEntity ToFake(this IEntity entity) {
    return new FakeEntity {
        team = entity.team,
        x = entity.gridPos.x,
        moveSpeed = Mathf.Max(0f, entity.GetRealMoveSpeed()),
        range = Mathf.Max(0f, entity[ST.Range]),
        danger = Mathf.Max(0f, entity.GetAssessPoint(APType.Danger)),
        survivability = Mathf.Max(0f, entity.GetAssessPoint(APType.Defend)),
    };
}
```

Usage: `entity.ToFake()` — 1 line per entity, no interface change needed.

If IBattleCard needs similar treatment, add `card.GetSpawnedEntity().ToFake()` (assuming IBattleCard exposes the entity it will spawn).

## Decision recorded

2026-05-18: Current converter is generic and correct. No concrete-type branching. Keep as-is. Future work only if new stats (e.g., attack speed, effect list) are needed in the simulation model.
