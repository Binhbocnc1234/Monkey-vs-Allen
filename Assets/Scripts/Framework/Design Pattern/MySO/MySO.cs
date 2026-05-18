using UnityEngine;
using System.Collections.Generic;
using System;


public abstract class MySO : ScriptableObject {
    // private static List<T> container = new List<T>();

    [ReadOnly] public string id;

    protected virtual void Awake() {
        // Generate only once
        if(string.IsNullOrEmpty(id)) {
            id = Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this); // mark asset dirty so ID is saved
#endif
        }
    }
}
// public abstract class NameIndexedSO : MySO{
//     protected override void Awake() {
//         id = name;
//     }
// }