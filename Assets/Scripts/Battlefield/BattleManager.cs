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
    private IGrid grid;
    private List<Entity> demoEnemies = new List<Entity>();
    void Start() {
        Initialize();
    }
    void Update() {
        if (BattleInfo.gameState == GameState.Fighting) {
            BattleInfo.timeElapsed += Time.deltaTime;
            AIManager.Ins.Update(Time.deltaTime);
            AlienResourceManager.Ins.Update(Time.deltaTime);
        }
    }
    public void Initialize() {
        var levelSO = BattleInfo.levelSO;
        if(defaultLevelSO != null) {
            Debug.Log("[BattleManager] Using defaultLevelSO");
            BattleInfo.Initialize(defaultLevelSO);
            levelSO = defaultLevelSO;
            if(levelSO.name == "Free Play Config") CustomSceneManager.isFreePlay = true;
        }

        grid = new GridSystem();
        IGrid.Ins = grid;
        grid.Initialize(levelSO.gridWidth, levelSO.openLanes);

        var presenter = GridPresenter.Ins ?? FindFirstObjectByType<GridPresenter>();
        if (presenter != null) {
            presenter.InitializeGrid(grid);
        } else {
            Debug.LogError("[BattleManager] GridPresenter not found in scene!");
        }

        EContainer.Ins.Initialize();
        EContainer.Ins.ClearEntity();

        uiManager = UIManager.Ins;
        // Initialize enviroment
        PlaceInitializerMapSO placeMap = SORegistry.Get<PlaceInitializerMapSO>()[0];
        placeMap.initializers[levelSO.place].Execute(levelSO);

        CreateTarget();

        foreach(LevelInitializerSO initializerSO in levelSO.levelInitializerSOs) {
            initializerSO.Execute(levelSO);
        }

        if(levelSO.modifier != null) {
            Instantiate(levelSO.modifier);
        }

        //Prize: reward after finishing level
        Prize.Ins.gameObject.SetActive(false);
        ShowEnemy();
        SingletonRegister.Get<OwnedCardManager>().SetReferencedList(PlayerData.GetOwnedCard());
        SingletonRegister.Get<ChosenCardManager>().SetReferencedList(BattleInfo.levelSO.choosenCardsBySystem);
        uiManager.InitChoosingCard();

        BattleInfo.OnStateChanged += () => ChangeState(BattleInfo.gameState);
        BattleInfo.ChangeState(GameState.ChoosingCard);
    }
    /// <summary>
    /// // Place targetMonkeys, targetAliens, and set win/lose condition
    /// </summary>
    public void CreateTarget() {
        // for(int y = 0; y < IGrid.Ins.height; ++y) {
        //     if(IGrid.Ins.openLanes[y] == false) continue;
        //     IEntity tM = IEntityRegistry.Ins.CreateEntity(new EntitySetting { so = targetMonkey, x = 0, lane = y, team = Team.Left, level = 1 });
        //     if (!CustomSceneManager.isFreePlay) tM.OnEntityDeath += () => CheckLose(tM);
        //     IEntity tA = IEntityRegistry.Ins.CreateEntity(new EntitySetting { so = targetAllen, x = IGrid.Ins.width - 1, lane = y, team = Team.Right, level = 1 });
        //     if (!CustomSceneManager.isFreePlay) tA.OnEntityDeath += () => CheckWin(tA);
        // }
    }
    public void ChangeState(GameState state){
        if (state == GameState.Fighting){
            BattleCardManager.Ins.CreateBattleCard();
            SingletonRegister.Get<ChosenCardManager>().SetControlTeam(BattleInfo.chosenTeam);
            // if(!CustomSceneManager.isFreePlay) {
                AIManager.ResetInstance();
                AlienResourceManager.ResetInstance();
                AIManager.Ins.Initialize();
            // }
            ClearDemoEnemy();
            SlidingCamera.Ins.enable = true;
            TutorialManager.Ins.Initialize();
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
        StartCoroutine(Prepare());
    }
    IEnumerator Prepare(){
        yield return StartCoroutine(uiManager.PrepareForBattleCoroutine());
        BattleInfo.ChangeState(GameState.Fighting);
        yield return StartCoroutine(PrepareUI.Ins.Fighting());
    }
    void CheckLose(IEntity e){
        // if (BattleInfo.levelSO.le)
        if (EContainer.Ins.GetTargetCount(Team.Left) == 0){
            BattleInfo.ChangeState(GameState.GameOver);
        }
    }
    void CheckWin(IEntity e){
        if(EContainer.Ins.GetTargetCount(Team.Right) == 0){
            BattleInfo.ChangeState(GameState.Victory);
            Vector3 prizePos = IGrid.Ins.GridToWorldPosition(e.gridPos.x, e.lane);
            // Try to get actual world position from model/wrapper if available
            Prize.Ins.Initialize(e.model.GetPosition());
            uiManager.gameObject.SetActive(false);
            // PlayerData handling
            LevelSO so = BattleInfo.levelSO;
            PlayerData.CompleteCampainLevel(so.number);
        }
    }
    IEnumerator GameOver() {
        uiManager.gameObject.SetActive(false);
        EContainer.Ins.InActiveAll();
        yield return new WaitForSeconds(0.5f);
        GridCamera.Ins.SetTarget(grid.GridToWorldPosition(2, 2));
        GridCamera.Ins.ZoomUp(1.5f);
        yield return new WaitWhile(() => GridCamera.Ins.isMoving);
        StartCoroutine(uiManager.Lose());
    }
    public void ShowEnemy() {
        float leftBound = IGrid.Ins.GridToWorldPosition(IGrid.Ins.width, 0).x;
        float rightBound = IGrid.Ins.GridToWorldPosition(IGrid.Ins.width + 3, 0).x;
        // const float minDemoEnem
        foreach(IBattleCard card in BattleInfo.teamDict[Team.Right].cards) {
            for(int i = 0; i < 2; ++i) {
                IEntity e = EContainer.Ins.CreateEntity(new EntitySetting {
                    so = card.GetSO().entitySO, x = Random.Range(leftBound, rightBound),
                    lane = Random.Range(0, IGrid.Ins.width), team = Team.Right, level = 1
                });
                e.BecomeInActive();
                if(e is Entity entity) demoEnemies.Add(entity);
            }
        }
    }
    public void ClearDemoEnemy() {
        foreach(Entity e in demoEnemies) {
            e.Die();
        }
        demoEnemies.Clear();
    }
}
