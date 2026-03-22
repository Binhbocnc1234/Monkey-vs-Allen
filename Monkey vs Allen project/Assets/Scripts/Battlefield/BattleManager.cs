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
    [SerializeField] private EntitySO targetMonkey, targetAllen;
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
        grid = GridSystem.Ins;
        grid.Clear();
        grid.Initialize(BattleInfo.levelSO);

        EContainer.Ins.Initialize();
        EContainer.Ins.ClearEntity();

        uiManager = UIManager.Ins;
        GridCamera.Ins.Initialize(grid);
        // Initialize enviroment
        PlaceInitializerMapSO placeMap = SORegistry.Get<PlaceInitializerMapSO>()[0];
        placeMap.initializers[BattleInfo.levelSO.place].Execute(BattleInfo.levelSO);
        // Place targetMonkeys, targetAliens
        for(int y = 0; y < IGrid.Ins.height; ++y) {
            if(IGrid.Ins.openLanes[y] == false) continue;
            IEntity tM = EContainer.Ins.CreateEntity(targetMonkey, 1, y, Team.Player, 1);
            tM.OnEntityDeath += () => CheckLose(tM);
            IEntity tA = EContainer.Ins.CreateEntity(targetAllen, IGrid.Ins.width - 2, y, Team.Enemy, 1);
            tA.OnEntityDeath += () => CheckWin(tA);
        }

        foreach(LevelInitializerSO initializerSO in BattleInfo.levelSO.levelInitializerSOs) {
            initializerSO.Execute(BattleInfo.levelSO);
        }

        Instantiate(BattleInfo.levelSO.modifier);
        //Prize: reward after finishing level
        Prize.Ins.gameObject.SetActive(false);
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
            GetComponent<EnemyManager>().Initialize();
            GetComponent<PlayerCardManager>().InitializeForEnemy();
            GetComponent<PlayerCardManager>().InitializeForPlayer();
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
    void CheckLose(IEntity e){
        // if (BattleInfo.levelSO.le)
        if (EContainer.Ins.GetTargetCount(Team.Player) == 0){
            BattleInfo.ChangeState(GameState.GameOver);
        }
    }
    void CheckWin(IEntity e){
        if(EContainer.Ins.GetTargetCount(Team.Enemy) == 0){
            BattleInfo.ChangeState(GameState.Victory);
            Prize.Ins.Initialize(e.transform.position);
            UIManager.Ins.gameObject.SetActive(false);
            // PlayerData handling
            LevelSO so = BattleInfo.levelSO;
            PlayerData.CompleteCampainLevel(so.number);
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
