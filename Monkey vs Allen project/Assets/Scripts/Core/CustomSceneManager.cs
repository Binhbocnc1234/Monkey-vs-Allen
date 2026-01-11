using System.Collections;
using System.Collections.Generic;
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
    public static List<Rewardable> rewardables{ get; private set; }
    public static void Reset() {
        targetScene = "";
        newCardSO = null;
        rewardables = null;
    }
    public static void ChangeScene(SceneEnum sceneType) {
        switch(sceneType) {
            case SceneEnum.Lobby:
                ChangeScene("Lobby"); break;
            case SceneEnum.Collections:
                ChangeScene("Collections"); break;
            case SceneEnum.Battlefield:
                ChangeScene("Battlefield"); break;
            default:
                Debug.LogError("CustomSceneManager::ChangeScene: Invalid parameter sceneType"); break;
        }
    }
    public static void ToLobby() {
        ChangeScene("Lobby");
    }
    public static void ToBattleField(LevelSO so) {
        BattleInfo.Initialize(so);
        ChangeScene("Battlefield");
    }
    public static void ToNewCard(CardSO so) {
        newCardSO = so;
        ChangeScene("NewCard");
    }
    public static void ToPrize(List<Rewardable> rwd) {
        rewardables = rwd;
        ChangeScene("Prize");
    }
    static void ChangeScene(string name){
        targetScene = name;
        SceneManager.LoadSceneAsync("Loading");
    }
}
