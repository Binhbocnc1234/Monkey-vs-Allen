using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private LobbyLevelUI prefab;
    void Start() {
        foreach(Place place in Enum.GetValues(typeof(Place))) {
            bool[] progress = PlayerData.GetProgress(place);
            bool isSatisfied = progress.Count(e => e == true) >= 7;
            if(isSatisfied) {
                for(int i = 0; i < LevelSO.COUNT_FOREACH_PLACE; ++i) {
                    LevelSO so = LevelSO.GetLevelSO(place, i);
                    if(progress[i]) {
                        AddNewLevel(so, LobbyLevelUI.State.Completed);
                    }
                    else {
                        AddNewLevel(so, LobbyLevelUI.State.Incompleted);
                    }
                }
            }
            else {
                for(int i = 0; i < LevelSO.COUNT_FOREACH_PLACE; ++i) {
                    LevelSO so = LevelSO.GetLevelSO(place, i);
                    if(progress[i]) {
                        AddNewLevel(so, LobbyLevelUI.State.Completed);
                    }
                    else {
                        AddNewLevel(so, LobbyLevelUI.State.Incompleted);
                        break;
                    }
                }
                break;
            }
        }
    }
    void AddNewLevel(LevelSO so, LobbyLevelUI.State state) {
        Instantiate(prefab, parent).ApplyLevelSO(so, state);
    }
    public void Return(){
        this.gameObject.SetActive(false);
        MyCamera.Ins.Reset();
    }

}
