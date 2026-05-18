using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake() {
        base.Awake();
        
        DontDestroyOnLoad(this);
    }
    public void StartBattle(LevelSO so){
        SceneManager.LoadScene("Battlefield");
        // BattleManager.Ins.Initialize(so);
    }
}
