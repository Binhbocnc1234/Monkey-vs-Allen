---
type: technical
audience: [developer, agent]
status: active
language: en
description: Documents the Assets/Resources folder structure, current runtime loading contract, architecture flaws, and recommended cleanup direction.
---

# Resources Folder

`Assets/Resources/` is the current runtime asset bucket for data, prefabs, sprites, animations, materials, localization assets, and scene-specific art.

Unity includes every asset under a `Resources` folder in player builds. Treat this folder as a runtime loading surface, not as a general asset storage location.

# Architecture Flaws

## `Resources` Has Too Many Responsibilities

`Assets/Resources/` mixes runtime data, entity art, UI art, environment art, tutorial assets, materials, animation clips, controllers, and miscellaneous prefabs.

This makes ownership unclear. A developer cannot tell whether an asset is loaded dynamically, referenced by GUID from another asset, or stored there only because the folder was convenient.

**Fix direction:** keep only assets that need `Resources` loading under `Resources`. Move GUID-referenced visuals to feature-owned folders such as `Assets/Art/`, `Assets/Prefabs/`, or the relevant feature folder.

## `SORegistry` Depends On A Broad Folder Scan

`SORegistry.Register<T>()` loads ScriptableObjects with:

```csharp
Resources.LoadAll<T>("Data")
```

This makes `Resources/Data` a hidden database. Any completed `MySO` asset under that folder can become runtime data if its type is registered.

Risks:
- accidental data enters runtime when placed under `Resources/Data`;
- startup cost grows as data grows;
- data ownership is folder-based instead of explicit;
- missing or duplicate singleton assets fail late at runtime.

**Fix direction:** keep `Resources/Data` as the only accepted dynamic-load area for now, but add explicit data registries later when data volume or validation needs grow.

## Addressables And `Resources` Overlap

Localization assets live under `Assets/Resources/Data/Localization`, while Addressables also tracks localization shared data by address.

This creates two loading ownership models for the same asset family. It can confuse build size, content update rules, and runtime expectations.

**Fix direction:** move localization assets to an Addressables-owned folder outside `Resources` when safe. Keep localization loading through Unity Localization and Addressables, not `Resources`.

## Visual Assets Are Probably Over-Included In Builds

Large visual folders such as `Monkey`, `Alien`, `Place`, `UI`, and `Lobby` are under `Resources`. Unity will include them in builds even if they are only referenced by prefabs or scenes.

This defeats Unity's normal dependency-based inclusion and makes build size harder to reason about.

**Fix direction:** audit visual assets by reference. Move assets that are only referenced by scenes, prefabs, or ScriptableObject fields out of `Resources`.

## `Other` And Empty Folders Hide Ownership

`Other` contains unrelated assets such as tutorial prefabs, chest assets, basic shapes, shadows, and empty object prefabs. Several folders under `Resources` are empty placeholders.

These names do not explain the owning feature or runtime contract.

**Fix direction:** move miscellaneous assets into named feature folders. Delete empty placeholders after confirming Unity references and `.meta` requirements.

## Asset Names Contain Temporary Or Ambiguous Labels

Some assets include labels such as `old`, `Modified`, `Test`, or duplicated `Variant`. Some names also include punctuation that is awkward for string paths.

This is risky in a `Resources` folder because path-like names often become API contracts when code starts using `Resources.Load`.

**Fix direction:** rename only after reference checks. Prefer stable names for assets that stay under `Resources`, because their path can become runtime API.

# Current Folder Structure

| Folder | Current role |
|---|---|
| `Data/` | ScriptableObject runtime data loaded through `SORegistry`, including cards, entities, effects, levels, localization, and singleton maps. |
| `Monkey/`, `Alien/`, `Tower/` | Entity and tower visual assets: sprites, prefabs, animation clips, controllers, bullets, and avatars. |
| `UI/`, `DeckBuilder/`, `FreePlay/` | UI prefabs, fonts, card art, icons, and deck/free-play UI assets. |
| `Place/`, `Lobby/` | Environment and lobby art. |
| `Effect/`, `Trick/`, `Materials/` | Visual effects, trick prefabs, shaders, and materials. |
| `Commons/`, `Other/` | Shared or miscellaneous assets with unclear ownership. |

# Runtime Contract

- `GameConfig.PlayModeInitialize()` registers the active ScriptableObject types before scene load.
- `SORegistry.Register<T>()` searches under `Resources/Data`.
- `PrefabRegisterSO` acts as a reference hub for commonly instantiated prefabs, sprites, and materials.
- `CustomUI` editor menu loads reusable UI prefabs directly from `Assets/Resources/UI/Prefab/` with `AssetDatabase`, not runtime `Resources.Load`.

# Cleanup Rules

- Do not add new assets to `Resources` unless runtime string loading needs them.
- Put new ScriptableObject data under `Resources/Data` only when it must be discovered by `SORegistry`.
- Prefer serialized references over `Resources.Load` paths.
- Prefer Addressables for assets that need dynamic loading, content updates, or localization ownership.
- Before moving existing assets, run a Unity reference scan and keep `.meta` GUIDs intact through Unity or safe filesystem moves.
