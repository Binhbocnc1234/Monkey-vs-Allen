---
type: technical

audience: [developer]
status: active
language: en
description: Defines technical design principles for modularity, readability, dependency direction, and Unity assembly organization.
related:
  - architecture
  - execution-paths
---

# Design Purposes

- **Reduce compile time.** An assembly (.asmdef) that other assemblies depend on must stay stable.
- **Readability.** Architecture should shout out what the project contains.
- **Minimal references between assemblies.** Fewer dependencies → easier bug fixing, faster recompilation.

# Hybrid Architecture

This architecture is neither purely layered (grouping by technical concerns) nor purely vertical (classification of assemblies by function). It combines both.

## Why hybrid

A pure layered architecture (Domain → Application → Infrastructure → Presentation → Bootstrap) causes expensive cascading recompiles:

- modify Domain file
- → recompile Domain
- → recompile Application
- → recompile Infrastructure
- → recompile Presentation
- → recompile Bootstrap

In game dev, domain code changes often. A feature-based grouping solves this:

- Entity
- Card
- Effect
- GridSystem
- AI
- UI

Modifying Effect → recompiles only Effect + its dependents. No cascade to unrelated features.

The hybrid approach uses feature-based asmdefs for volatile code, layered grouping only for stable cross-cutting concerns (Core, Framework).

# SOLID Principles Applied

These principles shape how assemblies and classes are organized.

| Principle | Application |
|-----------|-------------|
| **S**ingle Responsibility | Each assembly owns one concern: `MvA.Core` = contracts, `MvA.Entities` = entity logic, `MvA.UI` = rendering. A class should have one reason to change. |
| **O**pen/Closed | Entity behaviours are open for extension via `IBehaviour`/`EntityBehaviour` polymorphism. Add new behaviour by subclassing, not modifying existing code. |
| **L**iskov Substitution | Any `IBehaviour` subclass must be substitutable for its base. Behaviours that violate preconditions/postconditions break the entity system. |
| **I**nterface Segregation | Fine-grained interfaces: `IEntity` (stats/lifecycle), `IBattleCard` (card usage), `IEffect` (effect lifecycle), `IAssessable` (AI evaluation). No monolithic god interfaces. |
| **D**ependency Inversion | High-level modules (`MvA.Entities`, `MvA.AI`) depend on abstractions from `MvA.Core`/`MvA.Framework.Core`, not on concrete implementations. `MvA.Framework.Core` provides utilities, not game type definitions. |

# Assembly Partitioning Principles

Each `.asmdef` is a recompile boundary.

- **Frequently changed code** → own small asmdef (`MvA.Entities`, `MvA.Effect`, `MvA.AI`). Changes don't leak to unrelated assemblies.
- **Stable contracts** → `MvA.Core`, `MvA.Framework.Core`. Rarely change. Other assemblies depend on them safely.
- **Avoid circular dependencies.** If A depends on B, B must not depend on A.
- **Unity auto-generated assemblies** (`Assembly-CSharp.csproj`, etc.) should stay empty. All code must belong to a named `.asmdef`.

# SO-Driven Configuration

ScriptableObjects (SOs) define game data: `EntitySO`, `CardSO`, `TowerSO`, `SkillSO`, etc.

- SOs are **data containers**, not logic carriers.
- Logic lives in `MvA.Entities`, `MvA.Effect`, etc. — plain C# classes or thin MonoBehaviours.
- This enables rebalancing (tweak SO values) without recompiling logic assemblies.
- SOs are the single source of truth for entity stats, card costs, upgrade paths, and rarity.

# Event-Driven Layer Bridge

Communication between domain and UI follows an event-driven pattern:

- Domain assemblies (`MvA.Entities`, `MvA.Battlefield`) **publish events** when state changes (e.g. `OnEntityDeath`, `OnResourceChanged`, `OnShieldBroken`).
- UI assemblies (`MvA.UI`, `MvA.Lobby`) **subscribe** to these events and update the display.
- Domain code never references `UnityEngine.UI`, `TMPro`, or `MvA.UI` directly.

This satisfies the Phase 1 rule: "If display is needed, domain publishes events/state; UI assemblies subscribe and render."

# Pure C# Simulation

AI prediction (`Simulator.cs`) runs on `FakeEntity` — a POCO (Plain Old C# Object), not a `MonoBehaviour`.

- Simulation does **not** depend on Unity runtime (no Transform, no Instantiate, no Update loop).
- The converter from `IEntity`/`IBattleCard` → `FakeEntity` is **generic** — reads from `IEntity` interface only, no concrete-type checks. Adding new entity types does not change the converter.
- This enables running AI predictions off the main thread or in headless test environments.

# Dependency Direction Principles

- UI is the outermost layer (presentation). It only consumes data/events from domain.
- Domain/Core/Entities/Cards/Battlefield/Grid/Effect/Event must not depend on UI.
- Assemblies in inner layers must not reference assemblies in outer layers.
- Assemblies that contain contracts/interfaces/abstract types must stay stable, change rarely, and must not depend on rendering.

## Mandatory Rules (Phase 1)

- Do not add any new reference to `MvA.UI` in domain asmdefs.
- Do not add any new reference to `Unity.TextMeshPro` in domain asmdefs.
- Do not add UI/TMP namespaces in domain classes (e.g. `UnityEngine.UI`, `TMPro`).
- If display is needed, domain publishes events/state; UI assemblies subscribe and render.

## Prohibited New Edges

| From | To | Status |
|------|----|--------|
| MvA.Entities | MvA.UI | No new additions; target is removal |
| MvA.Cards | MvA.UI | No new additions; target is removal |
| MvA.Battlefield | MvA.UI | No new additions; target is removal |
| MvA.Cards | Unity.TextMeshPro | No new additions; target is removal |
| MvA.Battlefield | Unity.TextMeshPro | No new additions; target is removal |

## Phase 1 Pass/Fail Criteria

- PASS: No domain asmdef adds any new dependency to `MvA.UI` or `Unity.TextMeshPro`.
- PASS: New domain code does not use UI/TMP namespaces.
- FAIL: Any PR that adds a domain → UI/TMP edge without an approved exception.

## Temporary Exception Policy

- Exceptions are allowed only with a clear technical reason and a removal deadline.
- Every exception must include a migration issue and due date.
- No exception may expand its scope to other assemblies.

## Execution Notes

- Starting from Phase 1, every UI need in domain must go through event/adapter.
- Next objective (Phase 2): extract stable contracts to reduce fan-out from Core/Framework.
