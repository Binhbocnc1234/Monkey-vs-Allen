---
type: technical
audience: [developer, agent]
status: active
language: en
description: Documents the Assets/Resources runtime data structure, loading contract, and maintenance rules.
---

# Resources Folder

`Assets/Resources/` is the runtime data loading surface for ScriptableObject data.

Unity includes every asset under a `Resources` folder in player builds. Treat this folder as a runtime loading surface, not as a general asset storage location.

# Current Architecture

`Resources` is intentionally narrow:

- `Assets/Resources/Data/` contains runtime ScriptableObject data discovered by `SORegistry`.
- Visual assets, prefabs, materials, sprites, controllers, animations, and miscellaneous assets live under `Assets/GameAssets/`.
- Localization assets live under `Assets/Localization/` and are owned by Unity Localization and Addressables.
- Config files that are not loaded through `Resources` live under `Assets/GameAssets/Config/`.

## Explicit Data Loading

`SORegistry.Register<T>()` loads each ScriptableObject type from configured folders, not from the whole `Data` tree.

```csharp
Resources.LoadAll<T>(folder)
```

# Current Folder Structure

| Folder | Current role |
|---|---|
| `Data/Alien/` | Alien entity and card ScriptableObject data. |
| `Data/Monkey/` | Monkey entity and card ScriptableObject data. |
| `Data/Tower/` | Tower entity and card ScriptableObject data. |
| `Data/Skill/` | Skill ScriptableObject data. |
| `Data/Effect/` | Effect ScriptableObject data. |
| `Data/Level/` | Level and level-initializer ScriptableObject data. |
| `Data/CardFrameSO/` | Card frame ScriptableObject data. |
| `Data/PrefabRegister/` | Singleton prefab/material reference registry. |
| `Data/PlaceInitializerMap/` | Singleton place-initializer map. |

# Runtime Contract

- `GameConfig.PlayModeInitialize()` registers the active ScriptableObject types before scene load.
- `SORegistry.Register<T>()` searches only the configured folder list for the requested type.
- `PrefabRegisterSO` acts as a reference hub for commonly instantiated prefabs, sprites, and materials.
- `CustomUI` editor menu loads reusable UI prefabs directly from `Assets/GameAssets/UI/Prefab/` with `AssetDatabase`, not runtime `Resources.Load`.

# Cleanup Rules

- Do not add new assets to `Resources` unless runtime string loading needs them.
- Put new ScriptableObject data under `Resources/Data` only when it must be discovered by `SORegistry`.
- Add a configured `SORegistry` folder entry for each new `MySO` type that is loaded at startup.
- Prefer serialized references over `Resources.Load` paths.
- Prefer Addressables for assets that need dynamic loading, content updates, or localization ownership.
- Keep visual assets, prefabs, animation clips, controllers, sprites, fonts, shaders, and materials out of `Resources`.
