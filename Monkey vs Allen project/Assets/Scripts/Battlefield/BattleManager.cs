using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public enum CardSystemType {
    PvZ2,
    PvZHeroes
}

public class BattleManager : Singleton<BattleManager> {
    public LevelSO defaultLevelSO;
    public Entity targetMonkeyPrefab, targetAllenPrefab;
    public UIManager uiManager;
    private GridSystem grid;
    protected override void Awake() {
        base.Awake();
    }
    void Start(){
        Initialize();
    }
    public void Initialize() {
        if(defaultLevelSO != null) {
            Debug.Log("[BattleManager] Using defaultLevelSO");
            BattleInfo.Initialize(defaultLevelSO);
        }
        EContainer.Ins.Initialize();
        EContainer.Ins.ClearEntity();
        //Initialize grid: 2D array and fill array with cells
        grid = GridSystem.Ins;
        uiManager = UIManager.Ins;
        grid.Clear();
        grid.Initialize(BattleInfo.levelSO);
        GridCamera.Ins.Initialize(grid);
        //Place blocks and targetMonkeys, targetAllens
        LevelInitializer.Ins.Initialize(BattleInfo.levelSO, GridSystem.Ins);
        foreach(Entity targetMonkey in EContainer.Ins.GetTargetMonkey()){
            targetMonkey.OnEntityDeath += CheckLose;
        }
        foreach(Entity targetEnemy in EContainer.Ins.GetTargetEnemy()){
            targetEnemy.OnEntityDeath += CheckWin;
        }
        //Prize: reward after finishing level
        Prize.Ins.gameObject.SetActive(false);
        SingletonRegister.Register(new DeadlyManager());
        BattleInfo.OnStateChanged += () => ChangeState(BattleInfo.gameState);
        BattleInfo.ChangeState(GameState.ChoosingCard);
    }
    void Update() {
        
    }
    public void ChangeState(GameState state){
        if (state == GameState.ChoosingCard){
            GetComponent<EnemyManager>().ShowEnemy();
            SingletonRegister.Get<OwnedCardManager>().SetReferencedList(PlayerData.GetOwnedCard().ToList<CardSO>());
            SingletonRegister.Get<ChosenCardManager>().SetReferencedList(BattleInfo.levelSO.choosenCardsBySystem);
            StartCoroutine(uiManager.InitChoosingCardCoroutine());
        }
        else if (state == GameState.Prepare){
            StartCoroutine(Prepare());
        }
        else if (state == GameState.Fighting){
            gameObject.GetComponent<EnemyManager>().Initialize();
            gameObject.GetComponent<PlayerCardManager>().Initialize();
            EnemyManager.Ins.ClearDemoEnemy();
            GridCamera.Ins.canDraging = true;
            GetComponent<TutorialManager>().Initialize();
        }
        else if (state == GameState.Victory){
            EContainer.Ins.InActiveAll();
            uiManager.gameObject.SetActive(false);
        }
        else if(state == GameState.GameOver){
            StartCoroutine(GameOver());
        }
    }
    public void ChangeToPrepare(){
        BattleInfo.ChangeState(GameState.Prepare);
    }
    IEnumerator Prepare(){
        yield return StartCoroutine(uiManager.PrepareForBattleCoroutine());
        BattleInfo.ChangeState(GameState.Fighting);
        yield return StartCoroutine(PrepareUI.Ins.Fighting());
    }
    void CheckLose(Entity e){
        if (EContainer.Ins.GetTargetMonkey().Count == 0){
            BattleInfo.ChangeState(GameState.GameOver);
        }
    }
    void CheckWin(Entity e){
        if(EContainer.Ins.GetTargetEnemy().Count == 0){
            BattleInfo.ChangeState(GameState.Victory);
            Prize.Ins.Initialize(e.GetWorldPosition());
            UIManager.Ins.gameObject.SetActive(false);
            // PlayerData handling
            PlayerData.CompleteLevel(BattleInfo.levelSO);
        }
    }
    IEnumerator GameOver(){
        uiManager.gameObject.SetActive(false);
        EContainer.Ins.InActiveAll(); 
        yield return new WaitForSeconds(0.5f);
        GridCamera.Ins.SetTarget(grid.GetCell(2, 2).transform.position, 1);
        yield return new WaitWhile(() => GridCamera.Ins.isMoving);
        StartCoroutine(uiManager.Lose());
    }
}
