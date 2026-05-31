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
            // [Wrapper] Phase 4: so.prefab.GetComponent<EntityWrapper>()?.model.UpdateSO(so);
            Debug.LogWarning($"[EntitySOAssigner] Skipped UpdateSO for {so.name} — Entity is plain C# now. Use wrapper in Phase 4.");
        }
    } 
}


#endif