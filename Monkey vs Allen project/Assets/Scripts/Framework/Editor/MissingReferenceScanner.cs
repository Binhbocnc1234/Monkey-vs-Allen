#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class MissingReferenceScanner {
    [MenuItem("Tools/Check Missing References")]
    static void Scan() {
        foreach (var go in GameObject.FindObjectsOfType<GameObject>(true)) {
            var comps = go.GetComponents<Component>();
            foreach (var c in comps) {
                if (c == null) {
                    Debug.LogWarning($"Missing script on: {go.name}", go);
                    continue;
                }

                var so = new SerializedObject(c);
                var prop = so.GetIterator();
                while (prop.NextVisible(true)) {
                    if (prop.propertyType != SerializedPropertyType.ObjectReference) continue;
                    if (prop.name == "m_Script") continue;

                    // objectReferenceValue == null -> either unassigned (instanceID==0) or missing (instanceID != 0)
                    if (prop.objectReferenceValue == null) {
                        if (prop.objectReferenceInstanceIDValue != 0)
                            Debug.LogWarning($"Missing reference (deleted) in {go.name} → {c.GetType().Name}.{prop.displayName}", go);
                        else
                            Debug.LogWarning($"Unassigned reference in {go.name} → {c.GetType().Name}.{prop.displayName}", go);
                    }
                }
            }
        }
        Debug.Log("Scan Complete.");
    }
}
#endif
