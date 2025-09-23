#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string conditionField;
    public ShowIfAttribute(string conditionField)
    {
        this.conditionField = conditionField;
    }
}
//Hide a field if the condition is false, 
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var showIf = (ShowIfAttribute)attribute;
        var condition = property.serializedObject?.FindProperty(showIf.conditionField);
        if (condition != null && condition.propertyType == SerializedPropertyType.Boolean){
            if (condition.boolValue){
                
                EditorGUI.PropertyField(position, property, label);
                
            }
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var showIf = (ShowIfAttribute)attribute;
        var condition = property.serializedObject?.FindProperty(showIf.conditionField);

        if (condition != null && condition.propertyType == SerializedPropertyType.Boolean)
        {
            return condition.boolValue 
                ? EditorGUI.GetPropertyHeight(property, label, true) 
                : EditorGUIUtility.standardVerticalSpacing; // instead of 0f
        }
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif
