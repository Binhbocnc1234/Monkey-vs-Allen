using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Entity), editorForChildClasses:true)]
public class EntityEditor : Editor {
    public override void OnInspectorGUI() {
        Entity entity = (Entity)target;

        if(GUILayout.Button("Update SO")) {
            if(entity.so == null) {
                Debug.LogWarning("[EntityEditor] Cannot update because so is null.", entity);
            }
            else {
                Undo.RecordObject(entity, "Update Entity SO");
                entity.UpdateSO(entity.so);
                EditorUtility.SetDirty(entity);
            }
        }

        DrawDefaultInspector();
    }
}
