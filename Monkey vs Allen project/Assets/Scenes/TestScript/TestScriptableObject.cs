using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "bruhSO", menuName = "ScriptableObject/bruh")]
public class TestScriptableObject : ScriptableObject {
    void OnEnable() {
        Debug.Log("Test so Enable");
    }
    void Awake() {
        Debug.Log("Test so awake");
    }
}
