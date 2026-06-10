using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

// internal class SOReloader : AssetPostprocessor {
//     public static bool isLoaded = false;
//     static void OnPostprocessAllAssets(string[] imported, string[] deleted,
//         string[] moved, string[] movedFromPaths) {
            
//         foreach (string path in imported) {
//             var obj = AssetDatabase.LoadAssetAtPath<MySO>(path);
//             if (obj != null) {
//                 isLoaded = false;
//             }
//         }

//         foreach (string path in deleted) {
//             var obj = AssetDatabase.LoadAssetAtPath<MySO>(path);
//             if (obj != null) {
//                 isLoaded = false;
//             }
//         }
//     }
// }
public static class SORegistry {
    private static readonly Dictionary<Type, HashSet<MySO>> _instanceMap = new();
    private static readonly Dictionary<Type, string[]> _resourceFolders = new() {
        { typeof(CardSO), new[] { "Data/Monkey", "Data/Alien", "Data/Tower" } },
        { typeof(EnemyCardSO), new[] { "Data/Alien" } },
        { typeof(MonkeyCardSO), new[] { "Data/Monkey", "Data/Tower" } },
        { typeof(CardFrameSO), new[] { "Data/CardFrameSO" } },
        { typeof(EffectSO), new[] { "Data/Effect" } },
        { typeof(EntitySO), new[] { "Data/Monkey", "Data/Alien", "Data/Tower" } },
        { typeof(LevelSO), new[] { "Data/Level" } },
        { typeof(PrefabRegisterSO), new[] { "Data/PrefabRegister" } },
        { typeof(PlaceInitializerMapSO), new[] { "Data/PlaceInitializerMap" } },
        { typeof(SkillSO), new[] { "Data/Skill" } },
        { typeof(TowerSO), new[] { "Data/Tower" } },
    };

    public static void Register<T>() where T : MySO {
        Type t = typeof(T);
        _instanceMap[t] = new();
        foreach(T so in LoadConfiguredResources<T>()) {
            if(!so.IsCompleted()) {
                continue;
            }
            _instanceMap[t].Add(so);
        }
    }

    private static IEnumerable<T> LoadConfiguredResources<T>() where T : MySO {
        Type type = typeof(T);
        if(!_resourceFolders.TryGetValue(type, out string[] folders)) {
            Debug.LogError($"[SOContainer] No resource folder configured for Type: '{type.Name}'");
            yield break;
        }

        foreach(string folder in folders) {
            foreach(T so in Resources.LoadAll<T>(folder)) {
                yield return so;
            }
        }
    }

    public static List<T> Get<T>() where T : MySO {
        Type type = typeof(T);
        if(_instanceMap.TryGetValue(type, out HashSet<MySO> returnedList)) {
            return returnedList.Cast<T>().Where((e) => e != null).ToList();
        }
        else {
            Debug.LogError($"[SOContainer] Attempted to get unregistered Type: '{type.Name}'");
            return null;
        }
    }
    public static T Get<T>(string id) where T : MySO {
        Type type = typeof(T);
        if(!_instanceMap.TryGetValue(type, out HashSet<MySO> returnedList)) {
            // Register<T>();
            Debug.LogError($"[SOContainer] Attempted to get unregistered Type: '{type.Name}'");
        }

        foreach(T soInstance in returnedList) {
            if(soInstance.id == id) {
                return soInstance;
            }
        }

        Debug.LogError($"[SOContainer] Attempted to get instance of Type: '{type.Name}' but id does not exist");
        return null;
    }
    public static T GetSOByName<T>(string name) where T : MySO {
        Type type = typeof(T);
        if(!_instanceMap.TryGetValue(type, out HashSet<MySO> returnedList)) {
            Debug.LogError($"[SOContainer] Attempted to get unregistered Type: '{type.Name}'");
        }

        foreach(T soInstance in returnedList) {
            if(soInstance.name == name) {
                return soInstance;
            }
        }

        Debug.LogError($"[SOContainer] Attempted to get instance of Type: '{type.Name}' but id does not exist");
        return null;
    }
}
