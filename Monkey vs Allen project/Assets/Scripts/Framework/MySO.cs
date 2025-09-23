using UnityEngine;
using System.Collections.Generic;
using System;


public abstract class MySO<T> : ScriptableObject where T : MySO<T>
{
    public static List<T> container = new List<T>();
    [ReadOnly] public string id;

    private void OnEnable()
    {
        // Generate only once
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this); // mark asset dirty so ID is saved
#endif
        }
    }

    public static void ResetContainer()
    {
        container = new List<T>(Resources.LoadAll<T>("SO/" + typeof(T).Name.Replace("SO", "")));
        Debug.Log($"Successfully added {container.Count} {typeof(T)}");
    }

    public static T RetrieveAsset(string searchId)
    {
        foreach (var asset in container)
        {
            if (asset.id == searchId)
                return asset;
        }
        Debug.LogError("CardSO::RetrieveAsset return null");
        return null;
    }
}
