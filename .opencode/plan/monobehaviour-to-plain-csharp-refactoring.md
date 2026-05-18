---
type: technical
module: ai-system-architecture
version: 1.0
audience: developer
related: [architecture-to-be, design-principles]
---

# Plan: MonoBehaviour → Plain C# Refactoring (Entity + IBehaviour)

**TL;DR:** Chuyển IEntity và IBehaviour từ MonoBehaviour sang plain C# class để loại bỏ FakeEntity (dùng EntityModel trực tiếp trong Simulator), tối ưu performance, và giảm coupling với Unity.

**Phân tầng mục tiêu:**
- `EntityModel` (plain C#) — logic + data
- `Entity` (MonoBehaviour) — thin wrapper, chỉ keep Unity lifecycle + UI/animation
- `IBehaviourModel` (plain C#) — behaviour logic
- `IBehaviour` (MonoBehaviour) — thin wrapper

---

## Thiệt hại chi tiết

### Serialized Fields cần handle

| Component | Fields | Loại | Chi phí | Giải pháp |
|-----------|--------|------|--------|-----------|
| Attack/RangedAttack | `attackTimer`, `animationTimer` | [SerializeField] | LOW — timers recreated runtime | Remove serialization; tạo timer trong Model |
| Skill | `so`, `skillIndex`, `cooldownTimer` | PUBLIC | LOW — so ref bền vững | Inject SO qua constructor (SO Registry) |
| RangedAttack | `bulletPrefab`, `firePoint` | PUBLIC | MEDIUM | Migrate prefab ref → parameter/DI |
| SlimzAttack | `pokePrefab`, `skillSO` | [SerializeField] | MEDIUM | Tương tự; inject qua constructor |
| BananaTree | `state`, `bananaPrefab`, `cooldownTimer` | PUBLIC + [SerializeField] | MEDIUM | Track state trong Model |
| Entity.Stats | `UDictionary<ST, float>` | [SerializeField] | HIGH | Stats trong EntityModel |
| EffectController | Component ref | [SerializeField] | HIGH (khác assembly) | Refactor cùng nhưng tách phase |

### Break Points

| Dependency | Hiện tại | Sau refactor |
|-----------|----------|-------------|
| Prefab Inspector serialization | `[SerializeField] Field` | SO + Registry pattern |
| `GetComponent<IBehaviour>()` | Tìm behaviour trên GameObject | Behaviour đã có trong EntityModel.behaviours |
| `transform.position` | Direct access | `gridPos` (Vector2) — đã có |
| `MonoBehaviour.Update()` | Automatic | `model.UpdateBehaviours(dt)` — gọi manual |
| Editor scripts | EditorGUI, OnValidate() | Xoá (out of scope, làm sau) |

### Qui mô refactoring

- **Files cần thay đổi**: ~25 files
- **Effort**: 3–5 ngày (1 dev)
- **Zero logic thay đổi** — chỉ sắp xếp lại cách khởi tạo và dependency

---

## Kiến trúc Proposed

### Before (hiện tại)
```
IEntity (MonoBehaviour)
├── Entity (MonoBehaviour)
│   ├── Stats
│   ├── behaviours: IBehaviour[]
│   └── [Methods]
├── IBehaviour (MonoBehaviour)
│   ├── Attack (MonoBehaviour) — [SerializeField] timers
│   ├── RangedAttack (MonoBehaviour)
│   └── Skill (MonoBehaviour) — [SerializeField] so

Simulator.cs
└── FakeEntity (plain C#) ← conversion layer
```

### After (proposed)
```
EntityModel (plain C#) — logic + stats + behaviours
├── Stats: Dictionary<ST, float>
├── behaviours: List<IBehaviourModel>
└── Methods: GetAssessPoint(), GetRealMoveSpeed(), UpdateBehaviours(dt)…

Entity (MonoBehaviour) — thin wrapper
├── EntityModel model
├── properties expose model fields
└── Update() → model.UpdateBehaviours(Time.deltaTime)

IBehaviourModel (plain C#) — behaviour logic
├── AttackModel
├── RangedAttackModel
├── SkillModel
└── Constructor(entityModel, so, …)

IBehaviour (MonoBehaviour) — thin wrapper
├── IBehaviourModel model
└── UpdateBehaviour(dt) → model.UpdateBehaviour(dt)

Simulator.cs
└── EntityModel[] directly (no FakeEntity ✓)
```

---

## Implementation Steps

### Phase 1: Setup Model Base Classes (*Day 1*)

1. **Tạo `Assets/Scripts/Core/Model/EntityModel.cs`** (plain C#)
   - Fields: team, gridPos, stats (Dictionary<ST, float>), health, behaviours
   - Methods: logic từ IEntity (GetAssessPoint, GetRealMoveSpeed, TakeDamage, Die…)
   - Constructor: `EntityModel(entitySO, team, level, x, lane)`
   - Không MonoBehaviour, không GameObject refs

2. **Tạo `Assets/Scripts/Core/Model/IBehaviourModel.cs`** (interface / abstract)
   - Methods: `CanActive()`, `UpdateBehaviour(dt)`, `GetPriority()`, `GetAnimatorStateName()`
   - Reference: `EntityModel entity` (inject qua constructor)

3. **Tạo base `BehaviourModel : IBehaviourModel`** (plain C#)
   - Protected field: `protected EntityModel entity`
   - Helper: các utility method dùng chung

4. **Tạo `Assets/Scripts/Core/SO/EntityRegistry.cs`** (static)
   - Method: `GetEntitySO(string soId)` — load từ Resources
   - Cache: Dictionary để tránh load lại
   - *Note: cần init sớm (SceneLoader / SOManager)*

### Phase 2: Migrate Entity Subclasses → Model (*Days 1–2*)

5. **Entity (MonoBehaviour) → thin wrapper** — *parallel with step 6*
   - Holds `EntityModel model`
   - `OnEnable`: `model = new EntityModel(so, team, level, x, lane)`
   - Remove: logic code; keep: MonoBehaviour lifecycle, Transform access
   - Expose model fields qua properties (backward compat)

6. **BasicMonkey → BasicMonkeyModel** — *parallel with step 5*
   - Copy logic vào Model; không MonoBehaviour
   - Test: `new EntityModel(so, team, 0, 0) → Health = expected`

### Phase 3: Migrate IBehaviour Subclasses → Models (*Days 2–3*)

7. **AttackModel** — *depends on Phase 2*
   - Copy từ Attack.cs; remove MonoBehaviour
   - Inject: `EntityModel entity` + `attackSpeed` parameter (từ stats)
   - Timer: use plain C# Timer class (không Unity Timer)

8. **RangedAttackModel** — *depends on Phase 2*
   - Bullet prefab → `IBullet bulletPrefab` parameter
   - Constructor: accept bulletPrefab (interface, ko MonoBehaviour ref)

9. **SkillModel** — *depends on Phase 2*
   - Constructor: `SkillModel(EntityModel entity, SkillSO so, int skillIndex)`

10. **AttackBehaviour (MonoBehaviour)** — thin wrapper
    - Holds `AttackModel model`
    - `UpdateBehaviour(dt)` → `model.UpdateBehaviour(dt)`
    - Damage apply → `model.DealDamage(target)`

### Phase 4: Refactor Simulator — Remove FakeEntity (*Day 4*)

11. **Delete FakeEntity** — `Assets/Scripts/Battlefield/AI/Simulator.cs` (lines 11–20)
    - Xoá: `ConvertIEntitiesToFake()`, `ConvertCardsToFake()`, `CloneEntities()`

12. **Update Simulator entry points**
    - `EvaluateBundle(IEntity[],…)` → `EvaluateBundle(EntityModel[],…)`
    - `EvaluateBundle_2` → tương tự

13. **Update EnemyManager**
    - `BuildBestBundleDecision()` (line 112–152): extract `EntityModel` từ wrapper Entity
    - Convert IBattleCard → EntityModel via `EntityRegistry`

14. **Test Simulator**
    - Run SimulatorTests → should pass (chỉ thay đổi type, không logic)

### Phase 5: Entity Lifecycle Integration (*Day 4–5*)

15. **BattleManager.cs** — *depends on Phase 2–3*
    - `Entity[] entities` → keep as is (wrapper layer)
    - Add loop: `foreach (entity in entities) entity.model.UpdateBehaviours(dt)`

16. **Entity initialization chain**:
    ```
    Before: Instantiate(prefab).GetComponent<Entity>().Initialize(so, team, x, level)
    After:  Instantiate(prefab).GetComponent<Entity>().model = new EntityModel(so, team, x, level)
    ```

17. **Prefab cleanup**:
    - Xoá `[SerializeField] Timer` fields trong IBehaviour prefabs
    - Keep: SO refs, prefab refs (giờ là DI parameter)
    - Verify: tất cả prefabs load không lỗi serialization

### Phase 6: Testing & Migration (*Day 5*)

18. **Unit Tests mới**
    - `Assets/Tests/Editor/EntityModelTests.cs` — initialization, stats, damage
    - `Assets/Tests/Editor/BehaviourModelTests.cs` — behaviour update cycle

19. **Integration test** (manual):
    - Load battle scene → entities move/attack bình thường
    - AI decision loop vẫn chạy
    - Console: không serialization warnings

20. **Performance benchmark**:
    - Before: FakeEntity conversion overhead (~10–15%)
    - After: EntityModel direct; eliminate overhead

---

## Verification

| # | Cách verify | Expected |
|---|------------|----------|
| 1 | `dotnet test` hoặc Unity Test Runner | SimulatorTests pass + EntityModelTests pass |
| 2 | Load battle scene, spawn entities | Entities move/attack bình thường |
| 3 | AI decision loop | Enemy chọn card, spawn đúng |
| 4 | Console check | Không serialization warning/error |
| 5 | Benchmark: 100x Simulator.EvaluateBundle | After nhanh hơn 10–15% (không conversion) |

---

## Scope

**Included (trong phase này):**
- Entity + 5+ subclasses → EntityModel
- IBehaviour + 3+ subclasses → IBehaviourModel
- FakeEntity — xoá hoàn toàn
- Simulator refactor — nhận EntityModel trực tiếp
- Prefab cleanup — xoá timer serialization không cần

**Excluded (làm sau):**
- HoppingMove static field audit (đã ghi nhận) — không phải lúc này
- EffectController refactor (khác assembly, scope creep)
- Network serialization (out of scope)
- Editor tooling update (post-launch)
- Editor scripts (EditorGUI, OnValidate trên behaviour)

---

## Decisions

| Decision | Choice | Lý do |
|----------|--------|-------|
| Wrapper vs Full Migration | Giữ thin MonoBehaviour wrapper | Maintain compatibility với scene deserialization + event binding |
| DI Pattern | Constructor injection (SO Registry) | Thay thế Inspector serialization; cleaner |
| FakeEntity Removal | Xoá hoàn toàn | Simulator nhận EntityModel trực tiếp |
| SO Registry init | SceneLoader / SOManager.Initialize() | Cần init trước khi spawn entity |
| Timeline | 5 days, 1 dev | High priority; refactor thuần (zero logic change) |

---

## Further Considerations

1. **EntityRegistry init timing**: Khi nào load SO Registry? Recommendation: `SOManager.Initialize()` trong SceneLoader Awake. Bạn có scene initialization pattern không?
2. **Prefab variants**: Có prefab child variations (e.g., "Master Chef Variant") — cần verify serialization vẫn hoạt động sau cleanup.
3. **Wrapper backward compat**: Entity (MonoBehaviour) cần expose model fields qua properties để codebase cũ không break. Bạn muốn giữ interface IEntity hay xoá luôn?
