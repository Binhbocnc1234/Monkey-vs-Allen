#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

//Usage: [ReadOnly] <access modifier> <data type> <field's name>
public class ReadOnlyAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}

// Any script inheriting from ReadOnlyMonoBehaviour will show all fields grayed-out without [ReadOnly] attributes.
public class ReadOnlyMonoBehaviour : MonoBehaviour { }
[CustomEditor(typeof(ReadOnlyMonoBehaviour), true)]
public class ReadOnlyMonoBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        DrawDefaultInspector();
        GUI.enabled = true;
    }
}

#endif
