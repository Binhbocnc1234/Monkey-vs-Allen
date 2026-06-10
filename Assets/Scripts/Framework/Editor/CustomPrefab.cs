using UnityEditor;
using UnityEngine;

public class CustomUI
{
    public static string pathHead = "Assets/Resources/UI/Prefab/";
    [MenuItem("GameObject/UI/My Text (TMP)", false, 10)]
    static void CreateCustomText(MenuCommand command) {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathHead + "MyTextTMP.prefab");
        var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        GameObject parent = command.context as GameObject;
        if(parent != null)
            GameObjectUtility.SetParentAndAlign(obj, parent);

        Undo.RegisterCreatedObjectUndo(obj, "Create My Text TMP");
        Selection.activeObject = obj;
    }
    [MenuItem("GameObject/UI/My Button (TMP)", false, 11)]
    static void CreateCustomButton(MenuCommand command)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathHead + "MyButtonTMP.prefab");
        var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        GameObject parent = command.context as GameObject;
        if (parent != null)
            GameObjectUtility.SetParentAndAlign(obj, parent);

        Undo.RegisterCreatedObjectUndo(obj, "Create My Button TMP");
        Selection.activeObject = obj;
    }
}