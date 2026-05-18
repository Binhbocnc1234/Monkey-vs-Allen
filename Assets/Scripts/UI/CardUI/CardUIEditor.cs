#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardUIEditor))]
public class CardUIEditor : Editor
{
    CardUI ui;

    void OnEnable()
    {
        ui = (CardUI)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        // manual preview button
        if (GUILayout.Button("Force Preview"))
        {
            ui.Validate();
        }
    }
}
#endif