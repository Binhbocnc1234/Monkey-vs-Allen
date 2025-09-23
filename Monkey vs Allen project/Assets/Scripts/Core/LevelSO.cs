

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="NewLevelSO", menuName = "ScriptableObject/LevelSO")]
public class LevelSO : MySO<LevelSO>{
    public int index;
    public Place place;
    public Sprite thumbnail;
    public bool isCompleted = false, isVisible = true;
    public List<CardSO> enemies;
    public bool canChooseCard = true;
    public List<CardSO> choosenCardsBySystem = new List<CardSO>();
    public static List<LevelSO> visibleSOs;
    public static LevelSO GetLevelSO(Place place, int index){
        foreach(LevelSO so in container){
            if (so.place == place && so.index == index){
                return so;
            }
        }

        Debug.LogError($"LevelSO::GetLevelSO: Cannot find LevelSO with place: {place} and index: {index}");
        return null;
    }
    public static List<LevelSO> GetVisibleSO(){
        if (visibleSOs != null){
            return visibleSOs;
        }
        else{
            visibleSOs = new List<LevelSO>();
        }
        foreach(var so in container){
            if (so.isVisible){
                visibleSOs.Add(so);
            }
        }
        return visibleSOs;
    }
}