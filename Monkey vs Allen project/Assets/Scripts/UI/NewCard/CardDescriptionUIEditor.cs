#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardDescriptionUIEditor))]
public class CardDescriptionUIEditor : Editor
{
    CardDescriptionUI ui;

    void OnEnable()
    {
        ui = (CardDescriptionUI)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        // manual preview button
        if (GUILayout.Button("Force Preview"))
        {
            if (ui.so != null)
                ui.Initialize(ui.so);
        }
    }
}
#endif