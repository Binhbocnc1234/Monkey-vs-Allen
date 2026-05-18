

using UnityEngine;
using System.Collections.Generic;
using MackySoft.SerializeReferenceExtensions.Editor;
using System;

public enum GridInitializerEnum {
    Level_1,
    Level_2,
    Garden,
}

[CreateAssetMenu(fileName = "NewLevelSO", menuName = "ScriptableObject/LevelSO")]
public class LevelSO : MySO {
    public const int COUNT_FOREACH_PLACE = 10;
    public int number;
    public Place place;
    public Sprite thumbnail;
    public int initialBanana = 30;
    public int gridWidth = 18;
    public int alienMaximumUpgradeCnt = -1;
    public float alienWaitingTime = 0;
    public bool[] openLanes = new bool[6];
    public List<EnemyCardSO> enemies;
    public bool canChooseCard = true;
    public List<CardSO> choosenCardsBySystem = new();
    [SerializeReference, SubclassSelector]
    public List<Rewardable> rewardables;
    public List<Tutorial> tutorials;
    public List<LevelInitializerSO> levelInitializerSOs;
    public GameObject modifier;
    public static LevelSO GetLevelSO(Place place, int index) {
        foreach(LevelSO so in SORegistry.Get<LevelSO>()) {
            if(so.place == place && so.number == index) {
                return so;
            }
        }
        Debug.LogError($"LevelSO::GetLevelSO: Cannot find LevelSO with place: {place} and index: {index}");
        return null;
    }
    public int GetLaneCount() {
        int cnt = 0;
        foreach(bool state in openLanes) {
            if(state) { cnt++; }
        }
        return cnt;
    }
    public static int GetPlaceCount() => Enum.GetValues(typeof(Place)).Length;
}