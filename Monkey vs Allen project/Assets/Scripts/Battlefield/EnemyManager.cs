using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyManager: Singleton<EnemyManager>{
    private IGrid grid;
    private Timer spawnTimer;
    [ReadOnly] public int meteorIndex = 0;
    // public float meteoriteUpgradeDelay = 100, meteoriteUpgradeDiff = 60;
    // private Timer meteoriteTimer;
    // private float coolDownFactor = 1f;
    private List<Entity> demoEnemies = new List<Entity>();
    public void Initialize(){
        spawnTimer = new Timer(BattleInfo.levelSO.allenanaSpawnDelay, true);
        grid = GridSystem.Ins;
        foreach(EnemyCardSO so in BattleInfo.levelSO.enemies){
            BattleCard newCard = new BattleCard(so, Team.Enemy);
            BattleInfo.choosenEnemies.Add(newCard);
        }
    }
    void Update(){
        if(BattleInfo.gameState != GameState.Fighting) { return; }
        if(spawnTimer.Count()) {
            BattleInfo.ChangeAllenanaCnt(1);
        }
        for(int i = 0; i < BattleInfo.choosenEnemies.Count; ++i) {
            IBattleCard card = BattleInfo.choosenEnemies[i];
            card.Update();
            if(card.CanUseCard(new Vector2Int(0, 0))) {
                card.UseCard(new Vector2Int(grid.width - 1, PickLane(card)));
            }
        }
    }
    int PickLane(IBattleCard so){ //Cần phải xử lý
        float[] distances = new float[grid.height];
        int[] monkeysValue = new int[grid.height];
        var openLanes = GridSystem.Ins.GetOpenLanes();
        int res = openLanes[Random.Range(0, openLanes.Count)];
        float maxDanger = 10;

        for(int i = 0; i < grid.height; ++i) {
            if(GridSystem.Ins.openLanes[i] == false) { continue; }
            var entities = EContainer.Ins.GetEntitiesByLane(i);
            if(entities.Count == 0) { continue; }
            float danger = 0;
            distances[i] = 999f;
            foreach(Entity e in entities) {
                if(e.team == Team.Player) {
                    float dis = e.Distance(EContainer.Ins.GetTargetEnemy()[i]);
                    if(e.so.tribes.Contains(Tribe.Monkey)) {
                        danger += e.GetDangerPoint();
                    }
                    distances[i] = Mathf.Min(distances[i], dis) / grid.cellSize;
                }
                else if(e.team == Team.Enemy) {
                    danger -= e.GetDangerPoint();
                }
            }
            // Prioritize spawning Enemy at lane that Monkeys have approached
            danger += Mathf.Max(grid.width - distances[i] - 2, 0);
            if(danger > maxDanger) {
                maxDanger = danger;
                res = i;
            }
        }
        return res;
    }
    // void CreateEnemy(Entity prefab, int lane){
    //     Entity newEnemy = grid.GetCell(grid.width-1, lane).PlaceObject(prefab.gameObject).GetComponent<Entity>();
    //     newEnemy.Initialize();
    //     newEnemy.SetEntityState(EntityState.Walk);
    // }
    public void ShowEnemy(){
        float upperBound = IGrid.Ins.GridToWorldPosition(IGrid.Ins.width - 1, IGrid.Ins.height - 1).y;
        float lowerBound = IGrid.Ins.GridToWorldPosition(IGrid.Ins.width - 1, 0).y;
        float leftBound = IGrid.Ins.GridToWorldPosition(IGrid.Ins.width, 0).x;
        float rightBound = leftBound + GridSystem.Ins.cellSize*2;
        Vector2 RandomPosInBound(){
            return new Vector2(Random.Range(leftBound, rightBound), Random.Range(lowerBound, upperBound));
        }
        // const float minDemoEnem
        foreach(IBattleCard card in BattleInfo.choosenEnemies){
            for(int i = 0; i < 2; ++i){
                GameObject newEnemy = Instantiate(card.GetSO().entitySO.prefab, RandomPosInBound(), Quaternion.identity);
                Entity e = newEnemy.GetComponent<Entity>();
                e.SetEntityState(EntityState.InActive);
                SingletonRegister.Get<ShadowContainer>().Get().Initialize(e, e.laneIndex);
                demoEnemies.Add(newEnemy.GetComponent<Entity>());
            }
        }
    }
    public void ClearDemoEnemy(){
        foreach(Entity e in demoEnemies){
            e.Die();
        }
        demoEnemies.Clear();
    }
}