using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class GameConfig {
    // Những thiết lập dưới đây được sử dụng khi mà việc test thủ công quá khó mà
    // cần có sự can thiệp sâu vào hệ thống
    public static bool unlockFullCards = true;
    // public static bool isInitialized{ get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void PlayModeInitialize() {
        Debug.Log("[GameConfig] Initialize for PlayMode");
        Application.targetFrameRate = 60;
        SingletonRegister.Reset();
        CustomSceneManager.Reset();
        SORegistry.Initialize();
        PlayerData.Initialize();
        PlayerData.Load();
        LeanTween.reset();
        // BattleInfo.Initialize(LevelSO.GetLevelSO(Place.Garden, 1));
        if(unlockFullCards) {
            foreach(MonkeyCardSO so in SORegistry.Get<MonkeyCardSO>()) {
                PlayerData.GetCardDataById(so.id).Unlock();
            }
            foreach(EnemyCardSO so in SORegistry.Get<EnemyCardSO>()) {
                PlayerData.GetCardDataById(so.id).Unlock();
            }
            
        }
    }
}