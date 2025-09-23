using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EContainer{

    // List of entities for each lane (lane count: GameConstants.GRID_HEIGHT)
    public static List<List<Entity>> entities = new List<List<Entity>>(IGrid.Instance.height);
    public static List<GameObject> entityPrefabs = new List<GameObject>();
    public static GameObject blockPrefab;
    static EContainer() {
        // Initialize the list for each lane
        for(int i = 0; i < IGrid.Instance.height; i++) {
            entities.Add(new List<Entity>());
        }
    }

    // Returns a list of all alive entities in specified lane, removing nulls
    public static List<Entity> GetEntitiesByLane(int lane)
    {
        if(lane < 0 || lane >= IGrid.Instance.height){
            Debug.LogError("EntityContainer::AddEntity: invalid lane");
        }
        entities[lane].RemoveAll(e => e == null);
        return entities[lane];
    }
    public static void AddEntity(Entity entity, int lane){
        if(lane < 0 || lane >= IGrid.Instance.height){
            Debug.LogError("EntityContainer::AddEntity: invalid lane");
        }
        entities[lane].Add(entity);
    }
    // public static GameObject GetEntityPrefabByName(string name){
    //     foreach(GameObject e in entityPrefabs){
    //         if (e.get == name){
    //             return e;
    //         }
    //     }
    //     Debug.LogError("EntityContainer::GetEntityPrefabByName: Cannot find prefab with that name");
    //     return null;
    // }
}
