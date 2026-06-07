---
type: technical

audience: [developer]
status: draft
language: en
description: Draft refactoring plan for moving MonoBehaviour-dependent logic toward plain C# architecture.
related:
  - architecture-to-be
  - design-principles
---

# Plan: MonoBehaviour → Plain C# Refactoring (Entity + IBehaviour)

**TL;DR:** Refactor trực tiếp `IEntity`, `Entity`, `IBehaviour` và các subclass từ MonoBehaviour sang plain C# class. Giữ nguyên tên class. MonoBehaviour wrapper (`EntityWrapper`, `BehaviourWrapper`) là step cuối cùng. Mục tiêu: loại bỏ FakeEntity (dùng Entity plain trực tiếp trong Simulator), tối ưu performance, giảm coupling Unity.

**Quy tắc đặt tên:**
- Class plain C#: giữ nguyên `IEntity`, `Entity`, `IBehaviour`, `Behaviour`
- Class MonoBehaviour: thêm `Wrapper` suffix (`EntityWrapper`, `AttackWrapper`)
- Wrapper giữ lại interface `IEntity` để backward compat

**Cách tiếp cận:**
- Refactor ngay trên file hiện tại (không tạo file mới)
- Đổi `GetComponent<T>()` → `GetBehaviour<T>()`
- Dùng `EContainer` có sẵn làm registry (không tạo EntityRegistry mới)

---

## Thiệt hại chi tiết

### Serialized Fields

| Component | Fields | Loại | Chi phí | Xử lý |
|-----------|--------|------|--------|-------|
| Attack/RangedAttack | `attackTimer`, `animationTimer` | [SerializeField] | LOW — timers recreated runtime | Xoá attribute; tạo timer trong constructor |
| Skill | `so`, `skillIndex`, `cooldownTimer` | PUBLIC | LOW | Inject SO qua constructor |
| RangedAttack | `bulletPrefab`, `firePoint` | PUBLIC | MEDIUM | Inject qua constructor (interface) |
| SlimzAttack | `pokePrefab`, `skillSO` | [SerializeField] | MEDIUM | Inject qua constructor |
| BananaTree | `state`, `bananaPrefab`, `cooldownTimer` | PUBLIC | MEDIUM | Track state trong Entity (plain) |
| Entity.Stats | `UDictionary<ST, float>` | [SerializeField] | HIGH | Stats trong Entity (plain) — đổi sang Dictionary |
| EffectController | Component ref | [SerializeField] | HIGH (khác assembly) | Tách phase riêng |

**Chấp nhận mất dữ liệu serialize hiện tại** — số Entity ít, gắn lại được.

### Break Points

| Dependency | Hiện tại | Sau refactor |
|-----------|----------|-------------|
| Prefab Inspector serialization | `[SerializeField] Field` | SO + constructor injection |
| `GetComponent<IBehaviour>()` | Unity API | `GetBehaviour<T>()` (method mới) |
| `EntityBehaviour.GetComponent<IEntity>()` | MonoBehaviour pattern | Constructor injection (Entity entity) |
| `transform.position` | Direct access | `gridPos` (Vector2) — đã có |
| `MonoBehaviour.Update()` | Automatic | `entity.UpdateBehaviours(dt)` — gọi manual |
| Editor scripts | EditorGUI, OnValidate() | Xoá (làm sau) |

### Qui mô

- **Files**: ~25 files — **refactor tại chỗ**, không tạo mới
- **Effort**: 3–5 ngày (1 dev)
- **Logic không thay đổi** — chỉ thay đổi cách khởi tạo và dependency

---

## Kiến trúc Proposed

### Before (hiện tại)

```
MvA.Core assembly:
  IEntity (MonoBehaviour, abstract)
  └── Entity (MonoBehaviour)
      ├── Stats (UDictionary<ST, float>)
      ├── behaviours: IBehaviour[]
      ├── GetComponent<IBehaviour>()
      └── IBehaviour[] = GetComponents<IBehaviour>()

  EntityBehaviour (MonoBehaviour) — GetComponent<IEntity>()
  IBehaviour (MonoBehaviour, abstract)
  └── Attack (MonoBehaviour) — [SerializeField] timers
  └── RangedAttack (MonoBehaviour)
  └── Skill (MonoBehaviour) — [SerializeField] so

MvA.AI assembly:
  Simulator — FakeEntity (plain C# struct) ← conversion layer
  EnemyManager — IEntity[] → FakeEntity[]
```

### After (proposed)

```
MvA.Core assembly (plain C#):
  IEntity (abstract class — plain C#)
  └── Entity (plain C#)
      ├── Stats: Dictionary<ST, float>
      ├── behaviours: List<IBehaviour>
      ├── team, gridPos, health...
      └── GetBehaviour<T>()

  IBehaviour (abstract class — plain C#)
  └── Behaviour (abstract — plain C#)
      ├── protected Entity entity (injected via constructor)
      ├── CanActive(), GetPriority(), UpdateBehaviour(dt)…

  Behaviour subtypes (plain C#):
  ├── Attack, RangedAttack, Skill, Build, Move, Idle…

MonoBehaviour layer (dùng cho scene/prefab — làm cuối):
  EntityWrapper (MonoBehaviour) : IEntity (interface)
  └── Entity model (plain C#)
  └── Update() → model.UpdateBehaviours(Time.deltaTime)

  BehaviourWrapper (MonoBehaviour)
  └── IBehaviour model (plain C#)

MvA.AI assembly:
  Simulator — Entity[] directly (no FakeEntity ✓)
  EnemyManager — Entity[] directly ✓
```

---

## Implementation Steps

### ✅ Phase 1: Core Entity/IBehaviour → Plain C# (*Done*)

1. **IEntity.cs** — Mono → plain C# (bỏ MonoBehaviour/attributes, `UDictionary` → `Dictionary`, thêm `GetBehaviour<T>()`, `SetBehaviours()`)
2. **IBehaviour.cs** — Mono → plain C# (bỏ EntityBehaviour, thêm `_isEnable`, bỏ BehaviourData)
3. **EntityBehaviour.cs** — **Xoá**
4. **Entity.cs** — Mono → plain C# (constructor, `UpdateBehaviours(dt)`, `GetBehaviour<T>()`, bỏ Awake/Update/Destroy/transform)
5. **EffectController.cs** — Mono → plain C# (bỏ UpdateManager, tự quản effects, thêm Update/Reset/Flush)
6. **IEffectable.cs** — Thêm ProcessDamageOutput/Input/Taken, NotifyOnAssistOrKill
7. **Idle.cs** — Plain C# (bỏ `using UnityEngine`)
8. **IBehaviour subclasses (IInitialize classes)** — Refactor: BasicMonkey, BinaryStar, BasicAlien, StatueOfLiberty, Farmer, AlienWithHelmetInitializer
9. **EContainer.cs** — Gọi `UpdateBehaviours(dt)`, placeholder cho wrapper

### ✅ Phase 2: Simulator + Entity Subclasses Cleanup (*Done*)

1. **Simulator.cs** — FakeEntity → SimEntity; snapshot methods replace Convert methods
2. **Entity subclasses** — Yorn, Allen, Target (đã sạch, không MonoBehaviour); Glutton (xoá Awake); DonaldTrump (xoá Update)
3. **BattleManager.cs** — Fix `GetComponent<Entity>()` + `e.transform` → cast + `IGrid.GridToWorldPosition`
4. **AreaOfEffect.cs** — Comment out GetComponent (chờ wrapper)
5. **EntitySOAssigner.cs** — Comment out GetComponent (editor script; chờ wrapper)

### ✅ Phase 3: Migrate Behaviour Subclasses → Plain C# (*Done*)

12. **Attack** (`Assets/Scripts/Entities/Commons/Attack/Attack.cs`)
    - Refactor tại chỗ → plain C#
    - Timer dùng `deltaTime` (plain C#), không `Time.deltaTime`
    - `defender` kiểu `IEntity` (plain), constructor: `Attack(Entity entity)`

13. **RangedAttack** (`Assets/Scripts/Entities/Commons/Attack/RangedAttack.cs`)
    - Xoá MonoBehaviour; inject bulletPrefab + firePoint qua constructor

14. **Skill** (`Assets/Scripts/Entities/Skill.cs`)
    - Refactor tại chỗ; inject `SkillSO` qua constructor; xoá [SerializeField]

15. **BuildBehaviour** — parallel, tương tự

16. **Các behaviour còn lại** — Move, HoppingMove, StraightMove, BananaTree, InactiveBehaviour, ComfortRamenSkill

### Phase 4: Presentation layer for Entity

**Design assessment:**

The wrapper layer is already built in `Entities/Commons/Appearance/` (Model.cs, EntityAppearance, Die, TargetAppearance, WalkSynchronization, etc.). Model.cs acts as the central wrapper — it calls `AssignEntity(IEntity e)` to bind, subscribes to `OnHealthChanged`/`OnBehaviorActive`, handles sprite flipping, drives Animator, and initializes all `EntityAppearance` plugins.

**What needs to change:**

17. **EContainer.CreateEntity()** — currently creates `new Entity(so, ...)` without instantiating the prefab. Must:
    - Instantiate `so.prefab` at `IGrid.GridToWorldPosition(x, lane)`
    - Parent to `holder` transform
    - Get `Model` component from the instantiated prefab
    - Call `model.AssignEntity(entity)` to bind the pure C# Entity to its visual

18. **EntityDebugView** — empty shell exists, needs implementation to display Entity fields (stats, active behaviour, effects) in the Unity Inspector at runtime.

19. **Review existing Appearance scripts** — verify they work with plain C# Entity:
    - `Model.AssignEntity(IEntity)` ✅ already uses IEntity, no change needed
    - `Die.Initialize()` uses `model.e.OnEntityDeath` ✅ event-based, works fine
    - `WalkSynchronization.Update()` uses `Move.GetUnityMoveSpeed(e)` — verify Move is plain C#
    - `TargetAppearance` uses `e.GetHealthPercentage()` ✅ works fine
    - **Remove `PrefabRegistry.cs`** (empty), **remove `IPrefabRegistry` interface** from EContainer

**Why Entity.UpdateBehaviours stays in EContainer.Update() (not moved to Model):**
- Model is for visuals only (animation, position, effects)
- EContainer already iterates all entities each frame for cleanup (removing dead ones)
- Adding UpdateBehaviours to the same loop avoids an extra iteration
- Keeps Model focused on its single responsibility (appearance)

**Workload:**
- 1 file modified: `EContainer.cs` (CreateEntity logic + remove IPrefabRegistry refs)
- 1 file modified: `EntityDebugView.cs` (Inspector display)
- 1 file removed: `PrefabRegistry.cs`
- All Appearance scripts need review only (no rewrite)

### Phase 5: Integration & Serialization Fix (*depends on Phase 4*)

20. **Update BattleManager** — dùng wrapper
21. **Fix prefab scenes** — gắn lại SO refs + BehaviourWrapper refs

### Phase 6: Testing

22. **Unit Tests** — EntityTests.cs, BehaviourTests.cs, SimulatorTests.cs
23. **Integration test** — load scene, verify runtime

---

## Verification

| # | Cách verify | Expected |
|---|------------|----------|
| 1 | Unit Tests | EntityTests + BehaviourTests + SimulatorTests pass |
| 2 | Load battle scene | Entities spawn, move, attack bình thường |
| 3 | AI decision loop | EnemyManager + Simulator chạy (Entity plain) |
| 4 | Console | Không serialization warning/error |
| 5 | Benchmark 100x Simulator | ~10–15% faster (bỏ FakeEntity conversion) |
| 6 | Prefab re-serialization | Mọi prefab mở được trong Inspector |

---

## Scope

**Included:**
- Refactor tại chỗ: `IEntity`, `Entity`, `IBehaviour`, `Behaviour` → plain C#
- Entity subclass: BasicMonkey, Yorn, Slimz, BuilderMonkey…
- Behaviour subclass: Attack, RangedAttack, Skill, Build, Move, Idle…
- `GetComponent<T>()` → `GetBehaviour<T>()`
- Xoá `EntityBehaviour.cs`
- Xoá FakeEntity hoàn toàn
- Simulator + EnemyManager refactor
- `EntityWrapper`, `BehaviourWrapper` (MonoBehaviour — cuối cùng)
- `EContainer` update (dùng wrapper)

**Excluded (làm sau):**
- `HoppingMove` static field audit
- `EffectController` refactor (khác assembly)
- Network serialization (out of scope)
- Editor scripts (EditorGUI, OnValidate)

---

## Decisions

| Decision | Choice | Lý do |
|----------|--------|-------|
| **Refactor approach** | Tại chỗ (không tạo file mới) | Giữ nguyên tên, ít rename cost |
| **GetComponent** | `GetBehaviour<T>()` | Thay thế Unity API |
| **Registry** | `EContainer` có sẵn | Không tạo mới |
| **SORegistry** | Dùng thoải mái | User handle init |
| **Wrapper naming** | `EntityWrapper : IEntity` | Backward compat |
| **Wrapper timing** | Cuối cùng | Plain C# độc lập trước |
| **Serialization loss** | Chấp nhận | Số Entity ít, gắn lại được |
| **FakeEntity** | Xoá hoàn toàn | Dùng Entity (plain) trực tiếp |