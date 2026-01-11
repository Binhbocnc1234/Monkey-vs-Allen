

using UnityEngine;
using System.Collections.Generic;
using MackySoft.SerializeReferenceExtensions.Editor;

public enum GridInitializerEnum {
    Level_1,
    Level_2,
    Garden,
}

[CreateAssetMenu(fileName = "NewLevelSO", menuName = "ScriptableObject/LevelSO")]
public class LevelSO : MySO {
    public int difficulty;
    public int number;
    public Place place;
    public Sprite thumbnail;
    public int initialBanana = 6;
    public float allenanaSpawnDelay = 1;
    public int gridWidth = 18, gridHeight = 5;
    public bool isCompleted = false, isVisible = true;
    public List<EnemyCardSO> enemies;
    public bool canChooseCard = true;
    public List<CardSO> choosenCardsBySystem = new();
    [SerializeReference, SubclassSelector]
    public List<Rewardable> rewardables;
    public List<Tutorial> tutorials;
    public GridInitializerEnum initializer;
    public static List<LevelSO> visibleSOs;
    public static LevelSO GetLevelSO(Place place, int index) {
        foreach(LevelSO so in SORegistry.Get<LevelSO>()) {
            if(so.place == place && so.number == index) {
                return so;
            }
        }
        Debug.LogError($"LevelSO::GetLevelSO: Cannot find LevelSO with place: {place} and index: {index}");
        return null;
    }
    public static List<LevelSO> GetVisibleSO() {
        if(visibleSOs != null) {
            return visibleSOs;
        }
        else {
            visibleSOs = new List<LevelSO>();
        }
        foreach(var so in SORegistry.Get<LevelSO>()) {
            if(so.isVisible) {
                visibleSOs.Add(so);
            }
        }
        return visibleSOs;
    }
}

public static class LevelSOContainer {
    public static void Initialize() {
        foreach(var so in SORegistry.Get<LevelSO>()) {
            if(so.isVisible) {
                
            }
        }
    } 
}
