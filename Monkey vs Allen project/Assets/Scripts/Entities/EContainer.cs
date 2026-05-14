using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public interface IPrefabRegistry {
    public void CreatePrefab(IEntity e);
}
public class EContainer : IEntityRegistry {
    public new static EContainer Ins { get; private set; }
    public Transform holder;
    public EntitySO constructionMonkey, constructionAlien;
    private IPrefabRegistry prefabRegistry;
    // List of entities for each lane (lane count: GameConstants.GRID_HEIGHT)
    private List<List<Entity>> entities = new List<List<Entity>>();
    private List<Entity> targetMonkeys = new List<Entity>(), targetEnemies = new List<Entity>();
    protected override void Awake() {
        base.Awake();
        Ins = this;
    }
    public void Initialize(IPrefabRegistry prefabRegistry) {
        // Initialize the list for each lane
        for(int i = 0; i < IGrid.Ins.height; i++) {
            entities.Add(new List<Entity>());
        }
    }
    void Update() {
        for(int i = entities.Count - 1; i >= 0; i--) {
            var list = entities[i];
            entities[i].RemoveAll(e => e == null || e.IsDead());
        }
    }
    public override void CreateBuilder(EntitySetting set) {
        CreateBuilder(set.so, new Vector2Int((int)set.x, set.lane), set.team, set.level);
    }
    public override IEntity CreateEntity(EntitySetting set) {
        return CreateEntity(set.so, set.x, set.lane, set.team, set.level);
    }
    private void CreateBuilder(EntitySO tower, Vector2Int dest, Team team, int level) {
        EntitySO chosenBuilder = BattleInfo.chosenTeam == BattleInfo.monkeyInTeam ? constructionMonkey : constructionAlien;
        BuildBehaviour newBuilder = CreateEntity(
            chosenBuilder, team == Team.Left ? -1 : IGrid.Ins.width, dest.y, team).GetComponent<BuildBehaviour>();
        // BuildBehaviour newBuilder_2 = CreateEntity(
        //     chosenBuilder, team == Team.Player ? -2 : GridSystem.Ins.width + 1, dest.y, team).GetComponent<BuildBehaviour>();
        UnfinishedTower unfinishedTower = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().unfinishedTower).GetComponent<UnfinishedTower>();
        unfinishedTower.Initialize(new EntitySetting { so = tower, lane = dest.y, x = dest.x, level = level });
        newBuilder.Initialize(unfinishedTower, false);
        // newBuilder_2.Initialize(unfinishedTower, true);
    }
    public override IEntity CreateEntity(EntitySO so, float x, int lane, Team team, int level = 1) {
        if(so == null) {
            Debug.LogError("[EContainer] parameter 'so' is null!");
            return null;
        }
        else if(so.prefab == null) {
            Debug.LogError("[EContainer] so.prefab is null!");
            return null;
        }
        Vector3 worldPos = IGrid.Ins.GridToWorldPosition(x, lane);
        Entity e = Instantiate(so.prefab.GetComponent<Entity>(), worldPos, Quaternion.identity, holder);
        // List<string> unlockedSkills = so.unlockedSkillInFirstLevel.Select(e => e.name).ToList();
        e.Initialize(so, team, x, lane, level);
        // if (prefabRegistry != null) {
        //     prefabRegistry.CreatePrefab(e);
        // }
        foreach(Transform tr in e.transform)
        {
            if (tr.GetComponent<IModel>() != null) Destroy(tr.gameObject);
        }
        entities[lane].Add(e);
        if(e.GetSO().tribes.Contains(Tribe.Target)) {
            if(e.GetSO().tribes.Contains(Tribe.Monkey)) {
                targetMonkeys.Add(e);
            }
            else {
                targetEnemies.Add(e);
            }
        }
        Instantiate(SingletonRegister.Get<PrefabRegisterSO>().alienSpawningEffect, worldPos, Quaternion.identity, GeneralPurposeContainer.Ins.transform);
        SingletonRegister.Get<ShadowContainer>().Get().Initialize(e, e.lane);
        return e;
    }
    public override IEntity CreateEntity(EntitySO so, Vector2Int gridPos, Team team, int level = 1) {
        return CreateEntity(so, gridPos.x, gridPos.y, team);
    }
    public override IEntity CreateEntity(EntitySO so, int lane, Team team, int level = 1) {
        return CreateEntity(so, team == Team.Left ? -1 : IGrid.Ins.width, lane, team);
    }
    // Returns a list of all alive entities in specified lane, removing nulls
    public override IEntity[] GetEntitiesByLane(int lane, bool includeUntargetable = false) {
        if(lane < 0 || lane >= IGrid.Ins.height) {
            Debug.LogError("EntityContainer::AddEntity: invalid lane");
        }
        entities[lane].RemoveAll(e => e == null || e.IsDead());
        if(includeUntargetable) {
            return entities[lane].ToArray<IEntity>();
        }
        else {
            List<IEntity> preAns = new();
            foreach(IEntity e in entities[lane]) {
                if(e.isTargetable) preAns.Add(e);
            }
            return preAns.ToArray<IEntity>();
        }
    }
    public override IEntity[] GetEntities() {
        var result = new List<IEntity>();
        foreach(var group in entities)
            foreach(var e in group)
                if(e != null)
                    result.Add(e);
        return result.ToArray();
    }

    public void InActiveAll() {
        for(int i = 0; i < IGrid.Ins.height; ++i) {
            var entities = GetEntitiesByLane(i);
            foreach(Entity e in entities) {
                e.BecomeInActive();
            }
        }
    }
    public IEntity GetTargetEnemy(int lane) {
        foreach(var e in targetEnemies) {
            if(e.lane == lane) {
                return e;
            }
        }
        return null;
    }
    public override int GetTargetCount(Team team) {
        targetEnemies.RemoveAll(e => e == null || e.IsDead());
        targetMonkeys.RemoveAll(e => e == null || e.IsDead());
        if(team == Team.Left) {
            return targetMonkeys.Count;
        }
        else {
            return targetEnemies.Count;
        }
    }
    public void ClearEntity() {
        foreach(Transform child in transform) {
            if(!Application.isPlaying) {
                DestroyImmediate(child.gameObject);
            }
            else {
                Destroy(child.gameObject);
            }
        }
    }
    // public IEntity GetNearestEntity(IEntity entity) {
    //     IEntity result = entity;
    //     foreach(IEntity e in entities[entity.lane]) {
    //         if(e.Distance(entity) < result.Distance) {

    //         }
    //     }
    //     return result;
    // }
}
