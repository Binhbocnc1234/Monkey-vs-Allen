---
type: technical
audience: [developer, agent]
status: active
language: en
description: Describes the current Assets/Scripts architecture, assembly boundaries, Unity integration rules, and known gaps.
related:
  - design-principles
  - execution-paths
  - ai-system
---

# Scripts Architecture

`Assets/Scripts/` uses feature-oriented Unity assemblies. The `.asmdef` files are the source of truth for dependency maps, so this document describes responsibilities and boundaries instead of duplicating dependency edges.

The current architecture is hybrid:

- Stable contracts and utilities live in `Core/` and `Framework/`.
- Game features live in small feature folders such as `Entities/`, `Cards/`, `Effect/`, `Grid/`, `Bullet/`, and `AI/`.
- Presentation code lives in `UI/`, `Lobby/`, and appearance-specific assemblies.
- `Battlefield/` acts as the battle bootstrap and orchestration layer.

# Folder and Assembly Map

| Folder | Assembly | Responsibility |
|---|---|---|
| `Battlefield/` | `MvA.Battlefield` | Battle orchestration, managers, level setup, tutorial, free play, battle modifiers. |
| `Battlefield/AI/` | `MvA.AI` | AI decision logic, simulation, AI debug/editor partials. |
| `Bullet/` | `MvA.Bullet` | Bullet movement, collision, damage delivery, bullet behavior. |
| `Bullet/Appearance/` | `MvA.Bullet.Appearance` | Bullet visuals and bullet appearance adapters. |
| `Cards/` | `MvA.Cards` | Card runtime behavior and card effects. |
| `Core/` | `MvA.Core` | Game contracts, shared enums, ScriptableObject data types, scene flow, loading scene behavior, save/load, rewards. |
| `Effect/` | `MvA.Effect` | Status effects, passive skills, effect factories. |
| `Entities/` | `MvA.Entities` | Entity logic, movement, attacks, skills, health, entity data. |
| `Entities/Appearance/` | `MvA.Entities.Appearance` | Entity visuals, animation hooks, fill bars, model registry. |
| `Equipment/` | `MvA.Equipment` | Equipment type placeholders. Current files contain no gameplay logic. |
| `Framework/Design Pattern/` | `MvA.Framework.Core` | Reusable utilities, events, timers, pooling, singleton helpers, data structures. |
| `Framework/GraphicEffect/` | `MvA.Framework.GraphicEffect` | Reusable non-UI graphic effects such as parallax, light rays, blink effects. |
| `Framework/Runtime/` | `MvA.Framework.Runtime` | Runtime framework helpers that should stay game-agnostic. |
| `Framework/UI/` | `MvA.Framework.UI` and bundled UI asmdefs | Reusable UI helpers, layout utilities, tooltip, rounded corner components. |
| `Framework/Editor/` | Unity editor assembly | Editor-only tools such as autosave, scanners, animation clip resolver, custom prefab menu items. |
| `Grid/` | `MvA.Grid` | Battle grid, cells, map themes, grid camera. |
| `Lobby/` | `MvA.Lobby` | Lobby scene behavior and lobby UI coordination. |
| `UI/` | `MvA.UI` | Player-facing UI for battle, cards, deck builder, almanac, inventory, pause, level UI. |

# Layer Responsibilities

## Stable Contracts

`MvA.Core` and `MvA.Framework.Core` should stay stable because many assemblies depend on them.

- `MvA.Core` owns game-facing contracts and shared data types.
- `MvA.Framework.Core` owns generic utilities that should not know game-specific concepts.
- Avoid moving volatile feature behavior into these assemblies.

## Domain and Feature Code

Feature assemblies own gameplay behavior for one concern.

- `MvA.Entities` owns concrete entity logic.
- `MvA.Cards` owns card behavior.
- `MvA.Effect` owns effects and passive skills.
- `MvA.Grid` owns grid rules and map data.
- `MvA.Bullet` owns projectile behavior.
- `MvA.AI` owns AI planning and simulation.

Feature assemblies may use `UnityEngine` types when they are part of runtime gameplay. They should not depend on UI assemblies unless the feature is explicitly presentation-facing.

## Presentation

Presentation code converts game state into visuals and input flows.

- `MvA.UI` owns player-facing UI.
- `MvA.Lobby` owns lobby scene presentation.
- `MvA.Entities.Appearance` and `MvA.Bullet.Appearance` isolate visuals from core entity and bullet behavior.
- `MvA.Framework.UI` owns reusable UI utilities, not game-specific screen flow.

## Bootstrap

`MvA.Battlefield` wires systems together for battle scenes. It can depend on many feature assemblies because it sits at the composition edge.

Keep bootstrap classes focused on initialization, sequencing, and coordination. Move reusable gameplay rules into the relevant feature assembly.

# Unity Integration

Unity is the hosting environment, not a separate architectural layer.

- `MonoBehaviour` is allowed for scene objects, entity behavior, UI, and runtime adapters.
- ScriptableObjects define data such as entities, cards, effects, levels, and upgrades.
- ScriptableObjects should stay data-focused. Put behavior in feature assemblies.
- Editor-only code must live under an `Editor/` folder or an editor-only asmdef.
- Runtime code must not be placed under `Editor/`.

# Known Gaps

- `MvA.Core` contains mixed responsibilities: contracts, ScriptableObject data, save/load, reward, scene flow, loading scene behavior, camera helper. Keep new volatile feature logic out of `Core/`; split only when one responsibility starts changing often or needs isolated tests.
- `MvA.UI` depends on `MvA.Grid`. This is acceptable for current battle UI, but new domain-to-UI dependencies should be avoided.
- `Equipment/` currently has placeholder classes only. Add dependencies only when real equipment behavior appears.

# Maintenance Rules

- Keep every runtime `.cs` file under a named `.asmdef`; avoid `Assembly-CSharp`.
- Do not duplicate dependency maps in documentation. Read `.asmdef` files for current references.
- Keep feature behavior near its feature assembly.
- Keep generic framework code free of game-specific concepts when practical.
- Update this document when folders, assemblies, or major responsibilities change.
