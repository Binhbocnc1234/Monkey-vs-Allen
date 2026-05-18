#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

// public class MySOReloaderEditor : EditorWindow {
    // [MenuItem("Tools/SO Manager")]
    // public static void Open() {
    //     GetWindow<MySOReloaderEditor>("SO Manager");
    // }

    // void OnGUI() {
    //     GUILayout.Label("ScriptableObject Tools", EditorStyles.boldLabel);

    //     if(GUILayout.Button("Reset All Containers")) {
    //         MySOReloader.ResetAllContainer();
            
    //     }
    // }
    
    // [MenuItem("Tools/ResetAllSOContainer")]
    // public static void ResetAllContainer() {
    //     MySOReloader.ResetAllContainer();

    // }
// }
#endif

// public class MySOReloader : AssetPostprocessor {
//     static void OnPostprocessAllAssets(string[] imported, string[] deleted,
//         string[] moved, string[] movedFromPaths) {
            
//         foreach (string path in imported) {
//             var obj = AssetDatabase.LoadAssetAtPath<MySO>(path);
//             if (obj != null) {
//                 isLoaded = false;
//                 ResetAllContainer();
//             }
//         }

//         foreach (string path in deleted) {
//             var obj = AssetDatabase.LoadAssetAtPath<MySO>(path);
//             if (obj != null) {
//                 isLoaded = false;
//                 ResetAllContainer();
//             }
//         }
//     }
//     public static bool isLoaded = false;
//     public static void ResetAllContainer() {
//         if(isLoaded) { return; }
//         SOContainer.Reset();
//         Type[] types = new Type[4] { typeof(CardFrameSO), typeof(MonkeyCardSO),
//                             typeof(EnemyCardSO), typeof(GameConfig) };
//         foreach(Type type in types) {
//             var returnedList = SOContainer.Get(type);
//             Debug.Log($"Instances count of {type} : {returnedList.Count}");
//         }
//         isLoaded = true;
//         Debug.Log("Successfully reset all container!");
//     }
// }
