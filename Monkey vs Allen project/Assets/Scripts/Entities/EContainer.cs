using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class EContainer : Singleton<EContainer>{
    public Transform holder;
    // List of entities for each lane (lane count: GameConstants.GRID_HEIGHT)
    public List<List<Entity>> entities = new List<List<Entity>>();
    private List<Entity> targetMonkeys = new List<Entity>(), targetEnemies = new List<Entity>();
    public void Initialize() {
        // Initialize the list for each lane
        for(int i = 0; i < IGrid.ARRAY_HEIGHT; i++) {
            entities.Add(new List<Entity>());
        }
    }
    void Update() {
        for (int i = entities.Count - 1; i >= 0; i--)
        {
            var list = entities[i];
            entities[i].RemoveAll(e => e == null || e.IsDead());
        }
    }
    public Entity CreateEntity(EntitySO so, Vector2Int gridPos, Team team) {
        if (so == null) {
            Debug.Log("[EContainer] parameter 'so' is null!");
        }
        Entity e = Instantiate(so.prefab.GetComponent<Entity>(), GridSystem.Ins.GridToWorldPosition(gridPos), Quaternion.identity, holder);
        e.Initialize(so, team);
        e.ReturnToDefaultState();

        entities[gridPos.y].Add(e);
        if(e.so.tribes.Contains(Tribe.Target)) {
            if(e.so.tribes.Contains(Tribe.Monkey)) {
                targetMonkeys.Add(e);
            }
            else {
                targetEnemies.Add(e);
            }
        }
        SingletonRegister.Get<ShadowContainer>().Get().Initialize(e, e.laneIndex);
        return e;
    }
    // Returns a list of all alive entities in specified lane, removing nulls
    public ReadOnlyCollection<Entity> GetEntitiesByLane(int lane) {
        if(lane < 0 || lane >= IGrid.ARRAY_HEIGHT) {
            Debug.LogError("EntityContainer::AddEntity: invalid lane");
        }
        return entities[lane].AsReadOnly();
    }
    public ReadOnlyCollection<Entity> GetEntities() {
        var result = new List<Entity>();
        foreach (var group in entities)
            foreach (var e in group)
                if (e != null)
                    result.Add(e);
        return result.AsReadOnly();
    }

    public void InActiveAll() {
        for(int i = 0; i < IGrid.ARRAY_HEIGHT; ++i) {
            var entities = GetEntitiesByLane(i);
            foreach(Entity e in entities) {
                e.SetEntityState(EntityState.InActive);
            }
        }
    }
    public void IdleAll() {
        for(int i = 0; i < IGrid.ARRAY_HEIGHT; ++i) {
            var entities = GetEntitiesByLane(i);
            foreach(Entity e in entities) {
                e.SetEntityState(EntityState.Idle);
            }
        }
    }
    public List<Entity> GetTargetMonkey() {
        targetMonkeys.RemoveAll((e) => e == null);
        return targetMonkeys;
    }
    public List<Entity> GetTargetEnemy() {
        targetEnemies.RemoveAll((e) => e == null || e.IsDead());
        return targetEnemies;
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
    
}
