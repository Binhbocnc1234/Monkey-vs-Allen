using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Active(){
        this.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    public void DeActivate() {
        this.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    public void SaveAndReturn() {
        DeActivate();
        CustomSceneManager.ToLobby();
    }
    public void Resign() {
        DeActivate();
        BattleInfo.ChangeState(GameState.GameOver);
    }
}
