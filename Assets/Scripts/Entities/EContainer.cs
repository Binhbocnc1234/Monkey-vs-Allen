using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class EContainer : IEntityRegistry {
    public new static EContainer Ins { get; private set; }
    public EntitySO constructionMonkey, constructionAlien;
    // List of entities for each lane (lane count: GameConstants.GRID_HEIGHT)
    private List<List<Entity>> entities = new List<List<Entity>>();
    private List<Entity> targetMonkeys = new List<Entity>(), targetEnemies = new List<Entity>();
    protected override void Awake() {
        base.Awake();
        Ins = this;
    }
    public void Initialize() {
        // Initialize the list for each lane
        for(int i = 0; i < IGrid.Ins.height; i++) {
            entities.Add(new List<Entity>());
        }
    }
    /// <summary>
    /// Processes all entity updates: removes dead entities and calls UpdateBehaviours on alive ones.
    /// Extracted from Update() to allow direct testing without entering PlayMode.
    /// </summary>
    public void TickEntities(float deltaTime) {
        for(int i = entities.Count - 1; i >= 0; i--) {
            entities[i].RemoveAll(e => e == null || e.IsDead());
            foreach(Entity entity in entities[i]) {
                entity.UpdateBehaviours(deltaTime);
            }
        }
    }
    void Update() {
        TickEntities(Time.deltaTime);
    }
    public override void CreateBuilder(EntitySetting set) {
        EntitySO chosenBuilder = BattleInfo.chosenTeam == BattleInfo.monkeyInTeam ? constructionMonkey : constructionAlien;
        float builderX = set.team == Team.Left ? -1 : IGrid.Ins.width;
        BuildBehaviour newBuilder = CreateEntity(new EntitySetting {
            so = chosenBuilder, x = builderX, lane = set.lane, team = set.team, level = set.level
        }).GetBehaviour<BuildBehaviour>();
        UnfinishedTower unfinishedTower = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().unfinishedTower).GetComponent<UnfinishedTower>();
        unfinishedTower.Initialize(set);
        newBuilder?.Initialize(unfinishedTower, false);
    }
    public override IEntity CreateEntity(EntitySetting set) {
        if(set.so == null) {
            Debug.LogError("[EContainer] parameter 'so' is null!");
            return null;
        }
        else if(set.so.prefab == null) {
            Debug.LogError("[EContainer] so.prefab is null!");
            return null;
        }

        Entity e = new Entity(set.so, set.team, set.x, set.lane, set.level, set.isSimulated);
        NotifyEntityCreated(e);

        entities[set.lane].Add(e);
        if(e.GetSO().tribes.Contains(Tribe.Target)) {
            if(e.GetSO().tribes.Contains(Tribe.Monkey)) {
                targetMonkeys.Add(e);
            }
            else {
                targetEnemies.Add(e);
            }
        }
        return e;
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
