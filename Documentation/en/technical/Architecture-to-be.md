---
type: technical
module: architecture-to-be

audience: [developer]
status: active
language: en
description: Describes the intended project architecture, target module boundaries, and migration direction from the current architecture.
related:
  - design-principles
  - architecture-as-is
  - execution-paths
---

# Project Architecture (System-to-Be)

The target architecture is a **hybrid** of layered and feature-based grouping.

Assemblies can freely use C# functionality, `MonoBehaviour`, `Transform`, and other utilities from `UnityEngine`, because C# and UnityEngine are constant and will not change in the future. The architecture does not fight Unity — it isolates Unity-dependent code at the outermost layers.

## Layer Overview

```
Stable Contracts Layer
    ├── MvA.Framework.Core       — Design patterns, utilities, attributes
    ├── MvA.Framework.Runtime     — Runtime framework (reserved)
    ├── MvA.Core                  — Game contracts (IEntity, IBattleCard, IEffect, enums, SO base types)
    │
Domain / Feature Layer (volatile, feature-based asmdefs)
    ├── MvA.Entities              — Concrete entity logic (BasicMonkey, behaviour tree, skills)
    ├── MvA.Cards                 — Card logic (BattleCard, TowerCard, Trick)
    ├── MvA.Effect                — Status effects (Stun, OnFire, Poisoning, Shield)
    ├── MvA.Grid                  — Battle grid logic
    ├── MvA.Event                 — Event definitions and event-driven flow control
    ├── MvA.AI                    — Opponent AI (decision logic, simulation)
    │
Presentation Layer
    ├── MvA.UI                    — Player-facing UI (almanac, battle UI, deck builder, inventory)
    ├── MvA.Lobby                 — Lobby / main menu
    ├── MvA.Framework.UI          — Reusable UI widgets (rounded corners, gradients, layouts)
    │
Infrastructure Layer
    ├── SaveAndLoad               — Player data persistence (in MvA.Core/SaveAndLoad/)
    ├── Factories                 — EntityFactory (creates prefabs from EntitySO)
    ├── AudioServices
    ├── Ads
    │
Bootstrap Layer
    └── MvA.Battlefield           — Orchestration (BattleManager, GameManager, LevelInitializer)
```

## Layer Rules

### Stable Contracts Layer
- Contains abstract classes, interfaces, enums, value objects, ScriptableObject base types.
- **No logic** — only contracts and signatures.
- **Stable** — rarely changes. Low recompilation cost.
- Other assemblies depend on this layer; it never depends on them.

### Domain / Feature Layer
- Implements contracts defined in Core.
- **Feature-based** — each assembly corresponds to one game feature.
- **Volatile** — changes often. Changes are isolated to one assembly + its direct dependents.
- Must not reference UI, TMPro, or any presentation assembly.
- Prefers plain C# objects. May use `UnityEngine` base types (`Vector3`, `MonoBehaviour` base class) but avoids rendering (`SpriteRenderer`, `Animator`, `UI.Image`).

### Presentation Layer
- Depends on `UnityEngine.UI`, `TextMeshPro`.
- Only consumes data/events from domain.
- Never reverse-depends on domain assemblies.
- MonoBehaviour stays thin — delegates business decisions to domain objects.

### Infrastructure Layer
- Bridges domain to Unity/hardware.
- `SaveAndLoad`, `AudioServices`, `Ads`, `EntityFactory`.

### Bootstrap Layer
- Depends on all assemblies. Contains very few scripts.
- Orchestrators (`BattleManager`, `GameManager`, `LobbyManager`) — determine initialization order and component wiring.

## How Unity Fits

Unity is not a layer — it is the **hosting environment**. The architecture wraps Unity, not the reverse.

```
Unity Engine         ← MonoBehaviour, Instantiate, Input, Physics
    ↓
Presentation         ← UI.Image, TextMeshPro, SpriteRenderer, Animator
    ↓ (events)
Domain               ← IEntity, EntityBehaviour, BattleManager (pure C# contracts + thin MonoBehaviours)
    ↓
Platform/Hardware    ← PlayerPrefs, file I/O, ads SDK
```

Key rules:
- Domain assemblies may depend on `UnityEngine` base namespace (Vector3, Mathf, MonoBehaviour base class) but NOT on `UnityEngine.UI`, `TMPro`, or any rendering module.
- `MonoBehaviour` is allowed in domain only as a **thin base** (e.g. `IEntity : MonoBehaviour` provides Unity lifecycle). Actual logic lives in plain C# classes or behaviours composed on the entity.
- Simulation (`Simulator.cs`) is **pure C#** — no `MonoBehaviour`, no `Transform`, no `GameObject`. This enables headless AI prediction.
- `ScriptableObject` is used as data containers only. Logic is never stored in SOs.

## Target Folder Structure

```
Scripts/
├── Battlefield/        ← Bootstrap + orchestration
│   ├── AI/             → MvA.AI (pure C#, no UnityEngine)
│   ├── BattleModifier/
│   ├── FreePlay/
│   ├── LevelInitializer/
│   ├── PrimalBreach/
│   └── Tutorial/
├── Cards/              → MvA.Cards
├── Core/               → MvA.Core (stable contracts)
│   ├── Card/
│   ├── Effect/
│   ├── Entity/
│   ├── Input/
│   ├── Reward/
│   ├── SaveAndLoad/
│   └── SO/
├── Effect/             → MvA.Effect
├── Entities/           → MvA.Entities
│   ├── Alien/
│   ├── Combat/
│   ├── Commons/
│   └── Monkey/
├── Equipment/          → (reserved, not yet an asmdef)
├── Event/              → MvA.Event
├── Framework/          → Stable utilities layer
│   ├── Design Pattern/ → MvA.Framework.Core
│   ├── GraphicEffect/  → MvA.Framework.GraphicEffect
│   ├── Runtime/        → MvA.Framework.Runtime
│   └── UI/             → MvA.Framework.UI
├── Grid/               → MvA.Grid
├── Lobby/              → MvA.Lobby
├── Other/
└── UI/                 → MvA.UI
    ├── Almanac/
    ├── BattleFieldUI/
    ├── CardUI/
    ├── DeckBuilder/
    └── NewCard/
```

Every subfolder under `Scripts/` that contains code must be backed by a named `.asmdef`. The `Assembly-CSharp` default should remain empty.

# Migration Phases

## Phase 1 — Dependency Direction Freeze

- Lock dependency direction: domain must not reference UI.
- Freeze current dependency graph. No new edge from domain to UI.
- Clean unused UI/TMP usings in non-UI assemblies.

## Phase 2 — Stable Contracts

- Create assembly contract stable (interfaces, abstract types, value objects that rarely change).
- Contract assembly contains no MonoBehaviour.
- Domain assemblies depend on contract instead of cross-referencing type definitions.

## Phase 3 — Isolate Presentation

- Isolate UI/TMP into presentation layer only.
- Domain publishes events/state; UI subscribes and renders.
- Create a pure-C# simulation assembly (no UnityEngine) for AI forecast and battle prediction.

## Phase 4 — Thin MonoBehaviour

- MonoBehaviour plays a thin adapter role. Animation/view consumes logic results, does not drive core logic.
- Reduce fan-out from Framework/Core by splitting frequently-changing parts into outer assemblies.

## Phase 5 — Architectural Enforcement

- Apply architectural checklist in PR reviews to prevent architectural drift.