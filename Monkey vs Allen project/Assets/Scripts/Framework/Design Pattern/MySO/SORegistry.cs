using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

internal class SOReloader : AssetPostprocessor {
    public static bool isLoaded = false;
    static void OnPostprocessAllAssets(string[] imported, string[] deleted,
        string[] moved, string[] movedFromPaths) {
            
        foreach (string path in imported) {
            var obj = AssetDatabase.LoadAssetAtPath<MySO>(path);
            if (obj != null) {
                isLoaded = false;
            }
        }

        foreach (string path in deleted) {
            var obj = AssetDatabase.LoadAssetAtPath<MySO>(path);
            if (obj != null) {
                isLoaded = false;
            }
        }
    }
}
public static class SORegistry {
    private static readonly Dictionary<Type, HashSet<MySO>> _instanceMap = new();
    public static void Initialize() {
        _instanceMap.Clear();
        MySO[] mySOs = Resources.LoadAll<MySO>("");
        foreach(MySO so in mySOs) {
            var t = so.GetType();
            if (!_instanceMap.ContainsKey(t))
                _instanceMap[t] = new();
            _instanceMap[t].Add(so);
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
}