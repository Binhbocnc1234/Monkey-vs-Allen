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
    public IPrefabRegistry prefabRegistry;
    private IGrid grid;
    private List<Entity> demoEnemies = new List<Entity>();
    void Start() {
        Initialize();
    }
    void Update() {
        if (BattleInfo.gameState == GameState.Fighting) {
            BattleInfo.timeElapsed += Time.deltaTime;
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

        grid = IGrid.Ins;
        grid.Clear();
        grid.Initialize(levelSO.gridWidth, levelSO.openLanes);

        EContainer.Ins.Initialize(prefabRegistry);
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
        for(int y = 0; y < IGrid.Ins.height; ++y) {
            if(IGrid.Ins.openLanes[y] == false) continue;
            IEntity tM = IEntityRegistry.Ins.CreateEntity(targetMonkey, 0, y, Team.Left, 1);
            if (!CustomSceneManager.isFreePlay) tM.OnEntityDeath += () => CheckLose(tM);
            IEntity tA = IEntityRegistry.Ins.CreateEntity(targetAllen, IGrid.Ins.width - 1, y, Team.Right, 1);
            if (!CustomSceneManager.isFreePlay) tA.OnEntityDeath += () => CheckWin(tA);
        }
    }
    public void ChangeState(GameState state){
        if (state == GameState.Fighting){
            BattleCardManager.Ins.CreateBattleCard();
            SingletonRegister.Get<ChosenCardManager>().SetControlTeam(BattleInfo.chosenTeam);
            EnemyManager.Ins.Initialize();
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
            Prize.Ins.Initialize(e.transform.position);
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
        GridCamera.Ins.SetTarget(grid.GetCell(2, 2).transform.position);
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
                IEntity e = EContainer.Ins.CreateEntity(card.GetSO().entitySO,
                    Random.Range(leftBound, rightBound), Random.Range(0, IGrid.Ins.width), Team.Right, 1);
                e.BecomeInActive();
                demoEnemies.Add(e.GetComponent<Entity>());
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
