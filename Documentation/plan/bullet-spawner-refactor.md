# Refactor Bullet Spawning — New MvA.Bullet Assembly + Centralized Spawner

## TL;DR
- Create **MvA.Bullet** assembly containing all bullet behaviour + `BulletSpawner`.
- Move bullet classes out of `MvA.Entities/Combat/` into `Assets/Scripts/Bullet/`.
- Update contracts in `MvA.Core` (`IBullet`, `BulletSpawnRequest`, `IBulletSpawner`).
- Refactor `RangedAttack`/`PreciseRangedAttack`/`LobAttack` to use `IBulletSpawner.Ins.Spawn(...)`.
- **MvA.Entities never references MvA.Bullet** — only knows abstractions from MvA.Core.

## Current Status (Before Phase 1)

- **Codebase Status:** `IBullet.cs` defines `BulletSpawnRequest` (struct), `IBulletSpawner`, `IBulletInstance` but **never implemented**. Bullet classes live under `Assets/Scripts/Entities/Combat/` (MvA.Entities assembly). `RangedAttack`/`PreciseRangedAttack` call `Object.Instantiate` directly. `LobAttack` is stubbed.
- **Current Assembly Dependency (relevant):**
  ```
  MvA.Core ← MvA.Entities
  ```
- **Bullet classes currently depend on:** `Model` (MvA.Entities), `EContainer` (MvA.Entities), `IEntity` (MvA.Core), `Team` (MvA.Core).
- **Existing `[Wrapper] Phase 4` comments** confirm the team already planned to move instantiation out of Attack classes.

## Reason for the plan
- Eliminate scattered `Instantiate` calls → one central spawner.
- Separate bullet behaviour into its own assembly for cleaner layering.
- Entities should only care about "fire bullet", not "how to instantiate a bullet".
- Enable future bullet types without touching Attack or Entities code.

## New Assembly Architecture


| Assembly | Contains | References |
|----------|----------|------------|
| MvA.Core | `IBullet`, `BulletSpawnRequest` (GameObject prefab), `IBulletSpawner` | MvA.Framework.Core, etc. |
| MvA.Entities | `Attack`, `RangedAttack`, `PreciseRangedAttack`, `LobAttack`, etc. | MvA.Core, MvA.Effect, MvA.Framework.Core |
| **MvA.Bullet** (new) | `Bullet` (abstract), `CollisionBullet`, `StraightBullet`, `AimBullet`, `LobBullet`, `BulletSpawner` | **MvA.Core**, **MvA.Entities** |

**Key constraint:** `MvA.Entities` never references `MvA.Bullet`. Both only know abstractions from MvA.Core.

---

## ✅ Phase 1: Create MvA.Bullet Assembly & Move Files

**Description:** Create the new folder and assembly definition. Move all bullet-related files from `Entities/Combat/` to the new location. Adjust namespaces/usings if any.

### ✅ Step: Create assembly & folder structure

- **Description:**
  - ✅ Create folder: `Assets/Scripts/Bullet/`
  - ✅ Create `Assets/Scripts/Bullet/MvA.Bullet.asmdef` with references: `MvA.Core`, `MvA.Entities`
  - ✅ Move files from `Assets/Scripts/Entities/Bullet/` to `Assets/Scripts/Bullet/`:

**Verification:** ✅ Unity imports the new assembly without errors. `dotnet build MvA.Bullet.csproj` passes.

---

## ✅ Phase 2: Update Contracts in MvA.Core

**Description:** Rewrite `IBullet.cs`: rename `IBulletInstance` → `IBullet`, update `BulletSpawnRequest` with the agreed fields (prefab is `GameObject`), add `Ins` to `IBulletSpawner`.

### ✅ Step: Rewrite `IBullet.cs`

- **Description:**
  - `BulletSpawnRequest` → `class` (was `struct`)

    | Field | Type | Notes |
    |-------|------|-------|
    | `prefab` | `GameObject` | Serializable in Unity inspector |
    | `position` | `Vector3` | Spawn world position |
    | `rotation` | `Quaternion` | Default `Quaternion.identity` |
    | `owner` | `IEntity` | The entity that fired |
    | `target` | `IEntity` | Nullable; used by homing/lob bullets |
    | `damage` | `float` | Raw damage value |
    | `lane` | `int` | Owner's lane |

  - `IBulletInstance` → `IBullet` with method: `void Initialize(BulletSpawnRequest request)`
  - `IBulletSpawner` → returns `IBullet`, adds:
    ```csharp
    IBullet Spawn(BulletSpawnRequest req);
    public static IBulletSpawner Ins { get; set; }
    ```

- **Affected Files:** `Assets/Scripts/Core/IBullet.cs`

- **Reference Patterns:** Current `IBullet.cs` structure.

- **Dependencies:** None

### ✅ Step: Delete `BulletContainer.cs`

- **Description:** Remove the now-unnecessary `BulletContainer` singleton. Bullet parenting is handled by `BulletSpawner.holder` (a serialized `Transform` field on the spawner itself).

- **Affected Files:** ✅ `Assets/Scripts/Core/BulletContainer.cs` (deleted)

- **Dependencies:** None

**Verification:** ✅ `dotnet build MvA.Core.csproj` passes.

---

## ✅ Phase 3: Bullet Implements IBullet (MvA.Bullet)

**Description:** Make the abstract `Bullet` class implement `IBullet`. Update each concrete bullet to override `Initialize(BulletSpawnRequest)`. Keep the old `Initialize(float, IEntity, ...)` methods as `protected` bridges or remove them.

### ✅ Step: `Bullet.cs` — implement `IBullet`

- **Description:**
  - Add `: IBullet` to class declaration.
  - Add `public virtual void Initialize(BulletSpawnRequest request)`:
    ```csharp
    public virtual void Initialize(BulletSpawnRequest request) {
        owner = request.owner;
        damage = request.damage;
        lane = request.lane;
        team = request.owner.team;
    }
    ```
  - Remove the old `protected virtual void Initialize(float, IEntity)` — its logic is now in the new method.
  - **The new method is `public virtual`** so subclasses can `override`.

- **Affected Files:** `Assets/Scripts/Bullet/Bullet.cs`

- **Dependencies:** Phase 1 (file moved), Phase 2 (new contract)

### ✅ Step: `StraightBullet.cs` — override new Initialize

- **Description:**
  ```csharp
  public override void Initialize(BulletSpawnRequest request) {
      base.Initialize(request);
      disappearRange = request.owner[ST.Range] * 1.2f;
      startX = request.position.x;
      direction = request.owner.team == Team.Left ? Vector3.right : Vector3.left;
  }
  ```
  - Remove the old `new void Initialize(float, IEntity)`.

- **Affected Files:** `Assets/Scripts/Bullet/StraightBullet.cs`

- **Dependencies:** Previous step

### ✅ Step: `AimBullet.cs` — override new Initialize

- **Description:**
  ```csharp
  public override void Initialize(BulletSpawnRequest request) {
      base.Initialize(request);
      target = request.target;
      targetPosition = target?.model?.GetPosition() ?? request.position;
  }
  ```
  - Remove the old `Initialize(float, IEntity, IEntity)`.

- **Affected Files:** `Assets/Scripts/Bullet/AimBullet.cs`

- **Dependencies:** Previous step

### ✅ Step: `LobBullet.cs` — override new Initialize + SetUpwardVec setter

- **Description:**
  ```csharp
  public override void Initialize(BulletSpawnRequest request) {
      base.Initialize(request);
  }
  public void SetUpwardVec(float value) { upwardVec = value; }
  ```
  - Uses a setter that `LobAttack` calls after spawn (Phase 5).

- **Affected Files:** `Assets/Scripts/Bullet/LobBullet.cs`

- **Dependencies:** Previous step

### ✅ Step: `CollisionBullet.cs` — no changes

- **Description:** No code changes needed. It uses `EContainer` (MvA.Entities) which is fine since MvA.Bullet references MvA.Entities.

- **Affected Files:** `Assets/Scripts/Bullet/CollisionBullet.cs`

- **Dependencies:** None

**Verification:** `dotnet build MvA.Bullet.csproj` passes.

---

## ✅ Phase 4: Create BulletSpawner (MvA.Bullet)

**Description:** Implement `IBulletSpawner` as a `MonoBehaviour` in the new assembly. It carries a serialized `holder` Transform as the parent for all spawned bullets.

### ✅ Step: Create `BulletSpawner.cs`

- **Description:**
  - New file: `Assets/Scripts/Bullet/BulletSpawner.cs`
  - `BulletSpawner : MonoBehaviour, IBulletSpawner`
  - Serialized field: `public Transform holder;` — parent transform for all spawned bullets (assign in inspector).
  - In `Awake()`:
    ```csharp
    void Awake() {
        IBulletSpawner.Ins = this;
        if (holder == null) holder = this.transform; // fallback
    }
    ```
  - `IBullet Spawn(BulletSpawnRequest req)`:
    ```csharp
    public IBullet Spawn(BulletSpawnRequest req) {
        if (req.prefab == null) return null;
        GameObject go = Object.Instantiate(req.prefab, req.position, req.rotation, holder);
        IBullet bullet = go.GetComponent<IBullet>();
        bullet?.Initialize(req);
        return bullet;
    }
    ```
  - Remove `// [Wrapper] Phase 4` comments.

- **Affected Files:** `Assets/Scripts/Bullet/BulletSpawner.cs` (new)

- **Reference Patterns:** The `holder` field works like `ObjectPool.parent` — a serialized Transform reference set in the inspector.

- **Dependencies:** Phase 2 (contracts), Phase 3 (Bullet implements IBullet)

### Step: Wire up BulletSpawner in scene

- **Description:** Add a GameObject with `BulletSpawner` to the scene. Assign its `holder` field to a dedicated empty child (e.g., "BulletHolder") or any existing transform.

- **Affected Files:** Scene `.unity` file (manual step)

- **Dependencies:** Previous step

**Verification:** ✅ `dotnet build MvA.Bullet.csproj` passes.

---

## ✅ Phase 5: Refactor Attack Classes (MvA.Entities)

**Description:** Replace `Object.Instantiate` in each ranged attack with `IBulletSpawner.Ins.Spawn(...)`. Change the `bulletPrefab` field type from concrete bullet (`StraightBullet`, `AimBullet`) to `GameObject`. Subclasses that call `base.WhenAttackReady()` need no changes.

### ✅ Step: Refactor `RangedAttack.cs`

- **Description:**
  - Change field: `public StraightBullet bulletPrefab` → `public GameObject bulletPrefab`
  - Keep `public Transform firePoint`
  - `WhenAttackReady()`:
    ```csharp
    protected override void WhenAttackReady() {
        if (e.isSimulated) return;
        var request = new BulletSpawnRequest {
            prefab = bulletPrefab,
            position = firePoint.position,
            rotation = Quaternion.identity,
            owner = e,
            target = null,
            damage = e[ST.Strength],
            lane = e.lane
        };
        IBulletSpawner.Ins.Spawn(request);
    }
    ```
  - Remove `Object.Instantiate(...)` line.
  - Remove `// [Wrapper] Phase 4` comments.

- **Affected Files:** `Assets/Scripts/Entities/Commons/Attack/RangedAttack.cs`

- **Dependencies:** Phase 4 (spawner exists)

- **⚠ Prefab breakage:** Existing prefabs with `StraightBullet` assigned to `bulletPrefab` will lose their reference (field type changed). They need manual re-assignment of the same bullet prefab as a `GameObject`. **Low effort — one-time fix.**

### ✅ Step: Refactor `PreciseRangedAttack.cs`

- **Description:**
  - Change field: `public AimBullet bulletPrefab` → `public GameObject bulletPrefab`
  - Keep `public Transform firePoint`
  - `WhenAttackReady()`:
    ```csharp
    if (target == null || target.IsDead() || target.DistanceTo(e) > e[ST.Range])
        target = GetNearestPreyInRange();
    if (target == null) return;
    var request = new BulletSpawnRequest {
        prefab = bulletPrefab,
        position = firePoint.position,
        rotation = Quaternion.identity,
        owner = e,
        target = target,
        damage = e[ST.Strength],
        lane = e.lane
    };
    IBulletSpawner.Ins.Spawn(request);
    ```
  - Remove `Object.Instantiate(...)` and `GeneralPurposeContainer` parenting.
  - Remove `// [Wrapper] Phase 4` comments.

- **Affected Files:** `Assets/Scripts/Entities/Commons/Attack/PreciseRangedAttack.cs`

- **Dependencies:** Phase 4

### ✅ Step: Un-stub `LobAttack.cs`

- **Description:**
  - Uses spawner (same pattern as RangedAttack).
  - Trajectory computed inside `LobBullet.Initialize()` — no cast needed from Entities.
  - Removed `// [Wrapper] Phase 4` comments.

- **Affected Files:** `Assets/Scripts/Entities/Commons/Attack/LobAttack.cs`

- **Dependencies:** Phase 4

**Verification:**
- ✅ `dotnet build MvA.Core.csproj` — succeeded
- ✅ `dotnet build MvA.Entities.csproj` — succeeded
- ✅ `dotnet build MvA.Bullet.csproj` — succeeded
- `SlimzAttack`, `YornInfinityArrow` still work through `base.WhenAttackReady()` (no changes needed).

---

## Decisions / Exclusions

- **In scope:**
  - New assembly `MvA.Bullet` with moved bullet classes.
  - `IBullet`, `BulletSpawnRequest` (GameObject prefab), `IBulletSpawner` with `Ins`.
  - `Bullet : IBullet` with virtual `Initialize(BulletSpawnRequest)`.
  - `BulletSpawner` (singleton) in MvA.Bullet.
  - All three ranged attacks use the spawner.
  - Bullets parented to `BulletSpawner.holder` (serialized Transform, set in inspector).
  - MvA.Entities never references MvA.Bullet.

- **Out of scope / explicitly excluded:**
  - **No object pooling.**
  - **No changes to `Attack.cs` base class** — `e.isSimulated` branch stays.
  - **No changes to `AreaDamage.cs` or `StrikethroughAttack.cs`** — they don't spawn bullets.
  - **No changes to `SlimzAttack.cs` or `YornInfinityArrow.cs`** — they work through `base.WhenAttackReady()`.

## Risks / Open Questions

1. **LobBullet trajectory — Option 1 (self-compute) vs Option 2 (setter):** Should be decided during Phase 5 implementation. Low risk.
2. **Existing prefab references:** Attack prefabs with `StraightBullet`/`AimBullet` assigned to `bulletPrefab` will lose those references because the field type changes to `GameObject`. Must re-assign in the inspector. **Known, one-time cost.**
3. **Scene wiring:** `BulletSpawner` GameObject must be present in the scene. Assign its `holder` field to an empty child (e.g., "BulletHolder") or any existing transform.
