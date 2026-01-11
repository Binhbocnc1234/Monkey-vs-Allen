using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : ObjectPool<LobbyLevelUI>
{
    [HideInInspector] public List<LobbyLevelUI> levelGrid;
    void Start() {
        foreach(LevelSO so in SORegistry.Get<LevelSO>())
        {
            LobbyLevelUI newLevelUI = Get();
            if(PlayerData.CompletedLevels.Contains(so.id)) {
                newLevelUI.ApplyLevelSO(so, LobbyLevelUI.State.Completed);
            }
            else if(PlayerData.VisibleLevels.Contains(so.id)) {
                newLevelUI.ApplyLevelSO(so, LobbyLevelUI.State.Visible);
            }
            else {
                newLevelUI.ApplyLevelSO(so, LobbyLevelUI.State.Incompleted);
            }
        }
    }
    public void Return(){
        this.gameObject.SetActive(false);
        MyCamera.Ins.Reset();
    }

}
