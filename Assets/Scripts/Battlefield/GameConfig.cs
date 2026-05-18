using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class GameConfig {
    // Những thiết lập dưới đây được sử dụng khi mà việc test thủ công quá khó mà
    // cần có sự can thiệp sâu vào hệ thống
    public static bool unlockFullCards = true;
    public static int levelAvailable = 3;
    public static bool givePlayerShard = true;
    public static int overrideCampainProgress = 3;
    // public static bool isInitialized{ get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void PlayModeInitialize() {
        Debug.Log("[GameConfig] Initialize for PlayMode");
        Application.targetFrameRate = 60;
        BattleInfo.Reset();
        SingletonRegister.Reset();
        CustomSceneManager.Reset();
        // SORegistry
        SORegistry.Register<CardSO>();
        SORegistry.Register<EnemyCardSO>();
        SORegistry.Register<MonkeyCardSO>();
        SORegistry.Register<CardFrameSO>();
        SORegistry.Register<EffectSO>();
        SORegistry.Register<EntitySO>();
        SORegistry.Register<LevelSO>();
        SORegistry.Register<PrefabRegisterSO>();
        SORegistry.Register<PlaceInitializerMapSO>();

        SingletonRegister.Register(SORegistry.Get<PrefabRegisterSO>()[0]);
        PlayerData.Initialize();
        PlayerData.Load();
        if (overrideCampainProgress != -1) {
            for(int i = 0; i < overrideCampainProgress; ++i) {
                PlayerData.CompleteCampainLevel(i);
            }
        }
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
        if(givePlayerShard) {
            foreach(MonkeyCardSO so in SORegistry.Get<MonkeyCardSO>()) {
                PlayerData.GetCardDataById(so.id).shards = 200;
            }
            foreach(EnemyCardSO so in SORegistry.Get<EnemyCardSO>()) {
                PlayerData.GetCardDataById(so.id).shards = 200;
            }
        }
    }
}