#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public static class PlayerDataEditor {
    [MenuItem("Tools/LoadPlayerData")]
    public static void LoadInEditor() {
        PlayerData.Load();
    }
}
#endif