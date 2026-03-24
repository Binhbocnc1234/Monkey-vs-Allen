using System.Collections.Generic;
using System;
using UnityEngine;
public class TeamData {
    private int _resource;
    public event Action OnResourceChange;
    public int resource {
        get { return _resource; }
        set {
            _resource = value;
            OnResourceChange?.Invoke();
        }
    } 
    public List<IBattleCard> cards = new();
}
public static class BattleInfo {
    public static GameState gameState{get; private set;}
    public static LevelSO levelSO{ get; private set; }
    public const float CELL_SIZE = 2f;
    public static Dictionary<Team, TeamData> teamDict;
    public static List<CardSO> choosenCardSOs = new List<CardSO>(); // problem
    public static List<MonkeyCardSO> playerHand = new List<MonkeyCardSO>(); //for other mode
    public static event Action OnStateChanged;
    // public static 
    public static void Initialize(LevelSO so){
        OnStateChanged = null;
        teamDict = new();
        teamDict[Team.Player] = new();
        teamDict[Team.Enemy] = new();
        choosenCardSOs = new List<CardSO>();
        levelSO = so;
        if(!so.canChooseCard) {
            BattleInfo.choosenCardSOs = new List<CardSO>(so.choosenCardsBySystem);
        }
        teamDict[Team.Player].resource = so.initialBanana;
    }
    public static void ChangeState(GameState newState){
        Debug.Log($"BattleInfo::ChangeState: to {newState}");
        gameState = newState;
        OnStateChanged?.Invoke();
    }
}
public static class TechnicalInfo{
    public static float gravity = 10f;
    public const float speedMultiplier = 1.5f;
    public static bool isTutorial = false;
    public static MvAInputSystem controls;
    
    static TechnicalInfo(){
        controls = new MvAInputSystem();
        controls.Enable();
        Debug.Log("Successfully  reset TechnicalInfo's variables");
    }
}