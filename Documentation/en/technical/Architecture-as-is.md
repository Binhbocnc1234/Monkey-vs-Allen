---
type: technical
module: architecture-as-is
version: 1.0
audience: developer
related: [design-principles, architecture-to-be]
---

# Current Folder Structure

`Assets/Scripts/` maps to 14 named assemblies. The folder layout follows a flat feature-based grouping.

```
Scripts/
├── Battlefield/       → MvA.Battlefield
│   ├── AI/            → MvA.AI
│   ├── BattleModifier/
│   ├── FreePlay/
│   ├── LevelInitializer/
│   ├── PrimalBreach/
│   └── Tutorial/
├── Cards/             → MvA.Cards
│   └── Trick/
├── Core/              → MvA.Core
│   ├── Card/
│   ├── Effect/
│   ├── Entity/
│   ├── Input/
│   ├── Reward/
│   ├── SaveAndLoad/
│   └── SO/
├── Effect/            → MvA.Effect
│   └── PassiveSkill/
├── Entities/          → MvA.Entities
│   ├── Alien/
│   ├── Combat/
│   ├── Commons/
│   └── Monkey/
├── Equipment/         → (no asmdef — files fall into Assembly-CSharp)
│   ├── Attack/
│   ├── Data structures/
│   ├── Defense/
│   └── Magic/
├── Event/             → MvA.Event
├── Framework/         → Utility assemblies
│   ├── Design Pattern/ → MvA.Framework.Core
│   ├── Editor/         → (Unity Editor scripts)
│   ├── GraphicEffect/  → MvA.Framework.GraphicEffect
│   ├── Other/
│   ├── Runtime/        → MvA.Framework.Runtime
│   └── UI/             → MvA.Framework.UI
├── Grid/              → MvA.Grid
│   ├── Block/
│   └── MapTheme/
├── Lobby/             → MvA.Lobby
├── Other/
└── UI/                → MvA.UI
    ├── Almanac/
    ├── BattleFieldUI/
    ├── CardUI/
    ├── DeckBuilder/
    └── NewCard/
```

# Current Assembly Dependency Graph

## Foundation

```
MvA.Framework.Core          (no dependencies)
  │
  ├── MvA.Framework.Runtime (no dependencies)
  ├── MvA.Framework.UI      (deps: Framework.Core, Plugin.LeanTween, Unity.TextMeshPro)
  └── MvA.Framework.GraphicEffect (deps: Framework.Core)
```

## Core Contracts

```
MvA.Core                    (deps: Framework.Core, Unity.InputSystem, Plugin.LeanTween, MackySoft.SRE)
  │
  ├── MvA.Effect            (deps: Core, Framework.Core)
  ├── MvA.Grid              (deps: Core, Framework.Core)
  ├── MvA.Event             (deps: Core, Framework)
  ├── MvA.UI                (deps: Core, Grid, Framework.Core, Framework.UI, TMP, InputSystem, LeanTween)
  │
  ├── MvA.Cards             (deps: Core, Framework.Core, + 1 unresolved)
  ├── MvA.Lobby             (deps: Core, Framework.Core, UI, + 1 unresolved)
  │
  ├── MvA.Entities          (deps: Core, Effect, Framework.Core)
  │
  ├── MvA.AI                (deps: Core, Framework.Core, MackySoft.SRE)
  │
  └── MvA.Battlefield       (deps: Core, Grid, Entities, Cards, UI, Lobby, Effect, AI,
                             Framework.Core, Framework.UI, Framework.GraphicEffect,
                             LeanTween, TMP, MackySoft.SRE)
```

## Current Layer Mapping

| Layer | Assemblies | Notes |
|-------|-----------|-------|
| Stable Contracts | MvA.Core, MvA.Framework.Core | Rarely changes |
| Domain / Feature | MvA.Entities, MvA.Cards, MvA.Effect, MvA.Grid, MvA.Event, MvA.AI | Volatile; isolated in small asmdefs |
| Presentation | MvA.UI, MvA.Lobby, MvA.Framework.UI | Depends on domain (correct direction) |
| Infrastructure | Core/SaveAndLoad/, Core/Reward/, EntityFactory (inside Core/) | Mixed into Core, not separated |
| Bootstrap | MvA.Battlefield | Depends on everything; orchestrates |

## Notable Dependency Patterns

- `MvA.Battlefield` is the "hub" — it depends on almost every other assembly. This is intentional (bootstrap).
- `MvA.UI` depends on `MvA.Grid` — a domain assembly. This violates the "UI is outermost" ideal but is within Phase 1 tolerance (Grid is domain, not UI-to-UI).
- `MvA.Lobby` depends on `MvA.UI` — correct (lobby is presentation).
- `MvA.Entities` depends on `MvA.Effect` — correct (entities use effects).
- `MvA.AI` is clean — no domain → UI dependency. Uses `FakeEntity` POCO, not MonoBehaviours.

# Current Known Violations

These are documented issues that the migration phases aim to fix.

| Issue | Location | Impact |
|-------|----------|--------|
| Entity references Model directly | `IEntity.model` field | Couples entity logic to presentation. AI simulation bypasses Model, causing inconsistency. |
| UI scattered across assemblies | Battlefield/, Lobby/, UI/ | No single presentation boundary. Hard to enforce dependency rules. |
| Mixed concerns | `AlienWithHelmetInitializer.cs` | Contains both business logic and presentation logic in one file. |
| GameConfig lives in wrong place | GameConfig.cs (inside Battlefield/) | Should be in Bootstrap layer. |
| UI depends on singletons | TraitUI, Manager singletons | Hard to test, fragile ordering. |
| Equipment/ has no asmdef | `Assets/Scripts/Equipment/` | Files fall into default `Assembly-CSharp`, defeating asmdef isolation. |
| Unresolved GUIDs in asmdefs | MvA.Cards, MvA.Lobby | Two GUIDs reference assemblies no longer in the project. Build warning at minimum. |

# Unity Integration Notes

- `IEntity : MonoBehaviour` — entities are Unity GameObjects with lifecycle (Start, Update, OnDestroy). This is acceptable; the architecture does not aim to eliminate MonoBehaviour entirely.
- `EntityBehaviour` derives from `MonoBehaviour` — thin base for behaviour components.
- Many domain classes use `Vector3`, `Mathf`, `Transform` directly — acceptable per design principles.
- `Simulator.cs` is the **only pure-C# domain class** — no `MonoBehaviour`, no `GameObject`. This is the model for Phase 4's goal of isolating simulation.
- ScriptableObjects (`EntitySO`, `CardSO`, etc.) live in `Core/SO/` and define game data. Logic using SO data lives in domain assemblies (Entities, Cards, Effect).
