using System.Collections.Generic;
using System;
using UnityEngine;
public static class BattleInfo {
    public static GameState gameState{get; private set;}
    public static LevelSO levelSO{ get; private set; }
    public const float CELL_SIZE = 2f;
    public static int PlayerBaseHealth;
    public static int EnemyBaseHealth;
    public static int BananaCnt { get; private set; }
    public static int AllenanaCnt { get; private set; }
    public static List<CardSO> choosenCardSOs = new List<CardSO>();
    public static readonly List<IBattleCard> choosenEnemies = new (), chosenAllies = new();
    public static List<MonkeyCardSO> playerHand = new List<MonkeyCardSO>(); //for other mode
    public static event Action OnBananaChange;
    public static event Action OnStateChanged;
    public static void Initialize(LevelSO so){
        OnBananaChange = null;
        OnStateChanged = null;
        choosenEnemies.Clear();
        chosenAllies.Clear();
        choosenCardSOs = new List<CardSO>();
        // playerHand = new List<MonkeyCardSO>();
        BananaCnt = 0;
        AllenanaCnt = 0;

        levelSO = so;
        if (!so.canChooseCard){
            BattleInfo.choosenCardSOs = new List<CardSO>(so.choosenCardsBySystem);
        }
        ChangeBananaCnt(6);
    }
    public static void ChangeBananaCnt(int diff) {
        BananaCnt += diff;
        if(BananaCnt < 0) { BananaCnt = 0; }
        OnBananaChange?.Invoke();
    }
    public static void ChangeAllenanaCnt(int diff) {
        AllenanaCnt += diff;
        if (AllenanaCnt < 0){ AllenanaCnt = 0; }
    }
    public static void ChangeState(GameState newState){
        Debug.Log($"BattleInfo::ChangeState: to {newState}");
        gameState = newState;
        OnStateChanged?.Invoke();
    }
}
public static class TechnicalInfo{
    public static float gravity = 10f;
    public static bool isTutorial = false;
    public static MvAInputSystem controls;
    
    static TechnicalInfo(){
        controls = new MvAInputSystem();
        controls.Enable();
        Debug.Log("Successfully  reset TechnicalInfo's variables");
    }
}