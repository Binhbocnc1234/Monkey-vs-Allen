using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : Singleton<LobbyManagerUI>
{
    public RectTransform levelContainer;
    [HideInInspector] public List<LobbyLevelUI> levelGrid;
    void Start()
    {
        foreach(Transform levelUI in levelContainer) {
            LobbyLevelUI com = levelUI.GetComponent<LobbyLevelUI>();
            levelGrid.Add(com);
        }
        var visibleSOs = LevelSO.GetVisibleSO();
        for(int i = 0; i < visibleSOs.Count; ++i){
            levelGrid[i].ApplyLevelSO(visibleSOs[i]);
        }
    }
    public void Return(){
        this.gameObject.SetActive(false);
        MyCamera.Instance.Reset();
    }
}
