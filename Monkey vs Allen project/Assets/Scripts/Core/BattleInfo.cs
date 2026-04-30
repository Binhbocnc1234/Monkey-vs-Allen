using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
public class TeamData {
    private Team team;
    private int _resource;
    public event Action OnResourceChange;
    public int resource {
        get { return _resource; }
        set {
            if(BattleInfo.unlimitedBanana) {
                _resource = 3667;
            }
            else {
                _resource = value;
            }
            if(_resource < 0) Debug.LogError($"[TeamData] {team} has resource smaller than 0");
            OnResourceChange?.Invoke();
        }
    }
    public List<IBattleCard> cards = new();
    public List<CardSO> chosenCardSOs = new();
    internal TeamData(Team team) {
        this.team = team;
    }
}
public static class BattleInfo {
    public static GameState gameState{get; private set;}
    public static LevelSO levelSO { get; private set; }
    public const float CELL_SIZE = 2f;
    public static UDictionary<Team, TeamData> teamDict;
    private static Team _chosenTeam = Team.Left;
    public static event Action OnChosenTeamChanged;
    public static Team chosenTeam {
        get {
            return _chosenTeam;
        }
        set {
            _chosenTeam = value;
            OnChosenTeamChanged?.Invoke();
        }
    } // other meaning: controlled team by Player
    public static Team monkeyInTeam = Team.Left;
    public static List<MonkeyCardSO> playerHand = new List<MonkeyCardSO>(); //for other mode
    public static bool unlimitedBanana { get; private set; }
    public static bool noCooldown;
    public static float timeElapsed; // Thời gian trôi qua kể từ khi trận chiến bắt đầu
    public static event Action OnStateChanged;
    public const int resourcePerGeneration = 30, resourceDelay = 20;
    // public static 
    public static void Initialize(LevelSO so) {
        Reset();
        levelSO = so;
        if(!so.canChooseCard) {
            teamDict[Team.Left].chosenCardSOs = new List<CardSO>(so.choosenCardsBySystem);
        }
        teamDict[Team.Left].resource = so.initialBanana;
        teamDict[Team.Right].resource = so.initialBanana;
        teamDict[Team.Right].chosenCardSOs = so.enemies.ToList<CardSO>();
    }
    public static void Reset() {
        levelSO = null;
        OnStateChanged = null;
        teamDict = new();
        teamDict[Team.Left] = new(Team.Left);
        teamDict[Team.Right] = new(Team.Right);
        chosenTeam = Team.Left;
        unlimitedBanana = false;
        noCooldown = false;
        timeElapsed = 0;
    }
    public static void ChangeState(GameState newState) {
        Debug.Log($"[BattleInfo] Change atate: to {newState}");
        gameState = newState;
        OnStateChanged?.Invoke();
    }
    public static List<CardSO> GetChosenCardSOs() {
        return teamDict[chosenTeam].chosenCardSOs;
    }
    public static void ToggleUnlimitedBanana(bool toggle) {
        unlimitedBanana = toggle;
        if(unlimitedBanana) {
            teamDict[Team.Left].resource = 3667;
            teamDict[Team.Right].resource = 3667;
        }
        else {
            teamDict[Team.Left].resource = 0;
            teamDict[Team.Right].resource = 0;
        }
    }
    public static void ToggleCooldown(bool toggle) {
        noCooldown = toggle;
        if(toggle) {
            foreach(IBattleCard card in ICardContainer.Ins.GetBattleCards(Team.Left)) {
                card.cooldownTimer.SetCurTime(0);
            }
            foreach(IBattleCard card in ICardContainer.Ins.GetBattleCards(Team.Right)) {
                card.cooldownTimer.SetCurTime(0);
            }
        }
    }
}
public static class TechnicalInfo {
    public static float gravity = 10f;
    public const float speedMultiplier = 1.5f;
    public static bool isTutorial = false;
    public static MvAInputSystem controls;

    static TechnicalInfo() {
        controls = new MvAInputSystem();
        controls.Enable();
        Debug.Log("Successfully  reset TechnicalInfo's variables");
    }
}
