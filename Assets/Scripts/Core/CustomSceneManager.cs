using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneEnum{
    Lobby,
    Collections,
    Battlefield,
    Other,
}

public static class CustomSceneManager
{
    public static string targetScene { get; private set; }
    public static CardSO newCardSO { get; private set; }
    public static List<Rewardable> rewardables { get; private set; }
    public static bool isFreePlay = false;
    public static void Reset() {
        targetScene = "";
        newCardSO = null;
        rewardables = null;
        isFreePlay = false;
    }
    public static void ToLobby() {
        Reset();
        ChangeScene("Lobby");
    }
    public static void ToBattleField(LevelSO so) {
        Reset();
        BattleInfo.Initialize(so);
        ChangeScene("Battlefield");
    }
    public static void ToFreePlay() {
        Reset();
        isFreePlay = true;
        ChangeScene("BattleField");
    }
    public static void ToNewCard(CardSO so) {
        Reset();
        newCardSO = so;
        ChangeScene("NewCard");
    }
    public static void ToCollection() {
        Reset();
        ChangeScene("Collection");
    }
    public static void ToPrize(List<Rewardable> rwd) {
        Reset();
        rewardables = rwd;
        ChangeScene("Prize");
    }
    static void ChangeScene(string name){
        targetScene = name;
        SceneManager.LoadSceneAsync("Loading");
    }
}
