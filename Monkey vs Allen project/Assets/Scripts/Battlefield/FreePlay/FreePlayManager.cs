using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FreePlayManager : Singleton<FreePlayManager>{
    public LevelSO levelSO;
    public void Initialize() {
        BattleInfo.Initialize(levelSO);
    }
}