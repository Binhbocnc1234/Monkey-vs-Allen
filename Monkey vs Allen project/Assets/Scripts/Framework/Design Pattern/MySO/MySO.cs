using UnityEngine;
using System.Collections.Generic;
using System;


public abstract class MySO : ScriptableObject 
{
    // private static List<T> container = new List<T>();
    [ReadOnly] public string id;

    protected virtual void Awake()
    {
        // Generate only once
        if(string.IsNullOrEmpty(id)) {
            id = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this); // mark asset dirty so ID is saved
#endif
        }
    }
    // public static List<T> GetContainer() {
    //     return new List<T>(container);
    // }
    // public static void ResetContainer()
    // {
    //     container = new List<T>(Resources.LoadAll<T>(""));
    //     Debug.Log($"Successfully added {container.Count} {typeof(T)}");
    // }

    // public static T RetrieveAsset(string searchId)
    // {
    //     container.RemoveAll((e) => e == null);
    //     foreach (var asset in container)
    //     {
    //         if (asset.id == searchId)
    //             return asset;
    //     }
    //     Debug.LogError("CardSO::RetrieveAsset return null");
    //     return null;
    // }
}