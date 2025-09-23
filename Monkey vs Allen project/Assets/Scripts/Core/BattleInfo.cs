using System.Collections.Generic;
using System;
using UnityEngine;
public static class BattleInfo
{
    public static bool isDebugMode = true;
    public static GameState state = GameState.ChoosingCard;
    public static int level = 1;
    public static Place place = Place.Garden;
    public static LevelSO levelSO;
    public const int GRID_WIDTH = 18;
    public const int GRID_HEIGHT = 5;
    public const float CELL_SIZE = 2f;
    public static int PlayerBaseHealth;
    public static int EnemyBaseHealth;
    public static int BananaCnt{ get; private set; }
    public static List<CardSO> choosenCardSOs = new List<CardSO>();
    public static List<ICard> choosenCards = new List<ICard>();
    public static List<ICard> choosenEnemies = new List<ICard>();
    public static List<CardSO> playerHand = new List<CardSO>();
    public static event Action OnBananaChange;
    public static void Initialize(LevelSO so){
        OnBananaChange = null;
        choosenCards = new List<ICard>();
        choosenCards = new List<ICard>();
        choosenEnemies = new List<ICard>();
        playerHand = new List<CardSO>();
        BananaCnt = 0;
        
        BattleInfo.level = so.index;
        BattleInfo.place = so.place;
        if (!so.canChooseCard){
            BattleInfo.choosenCardSOs = new List<CardSO>(so.choosenCardsBySystem);
        }

        ChangeBananaCnt(6);
    }
    public static void ChangeBananaCnt(int diff){
        Debug.Log($"ChangeBananaCnt: {diff}");
        BananaCnt += diff;
        if(BananaCnt < 0) { BananaCnt = 0; }
        OnBananaChange?.Invoke();
    }

}
public static class TechnicalInfo{
    public static float gravity = 10f;
    public static MvAInputSystem controls;
    
    static TechnicalInfo(){
        controls = new MvAInputSystem();
        controls.Enable();
        Debug.Log("Successfully  reset TechnicalInfo's variables");
    }
}