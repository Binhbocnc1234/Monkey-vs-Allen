#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public static class EntitySOAssigner {
    [MenuItem("Tools/Assign and UpdateSO")]
    static void AssignAndUpdate() {
        SORegistry.Register<EntitySO>();
        foreach(EntitySO so in SORegistry.Get<EntitySO>()) {
            if(so.prefab == null) {
                continue;
            }
            Debug.Log(so.name);
            so.prefab.GetComponent<Entity>().UpdateSO(so);
        }
    } 
}


#endif