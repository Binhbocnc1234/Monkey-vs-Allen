using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyManager: Singleton<EnemyManager>{
    private IGrid grid;
    private Timer spawnTimer;
    [ReadOnly] public int meteorIndex = 0;
    private List<Entity> demoEnemies = new List<Entity>();
    public void Initialize(){
        spawnTimer = new Timer(BattleInfo.levelSO.allenanaSpawnDelay, true);
        grid = GridSystem.Ins;
    }
    void Update(){
        if(BattleInfo.gameState != GameState.Fighting) { return; }
        if(spawnTimer.Count()) {
            BattleInfo.teamDict[Team.Enemy].resource += 1;
        }
        for(int i = 0; i < BattleInfo.teamDict[Team.Enemy].cards.Count; ++i) {
            IBattleCard card = BattleInfo.teamDict[Team.Enemy].cards[i];
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
            if(entities.Length == 0) { continue; }
            float danger = 0;
            distances[i] = 999f;
            foreach(Entity e in entities) {
                if(e.team == Team.Player) {
                    if (EContainer.Ins.GetTargetEnemy(i) == null) {
                        continue;
                    }
                    float dis = e.DistanceTo(EContainer.Ins.GetTargetEnemy(i));
                    if(e.GetSO().tribes.Contains(Tribe.Monkey)) {
                        danger += e.GetDangerPoint();
                    }
                    distances[i] = Mathf.Min(distances[i], dis);
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
    public void ShowEnemy(){
        float leftBound = IGrid.Ins.GridToWorldPosition(IGrid.Ins.width, 0).x;
        float rightBound = IGrid.Ins.GridToWorldPosition(IGrid.Ins.width + 3, 0).x;
        // const float minDemoEnem
        foreach(IBattleCard card in BattleInfo.teamDict[Team.Enemy].cards){
            for(int i = 0; i < 2; ++i){
                IEntity e = EContainer.Ins.CreateEntity(card.GetSO().entitySO,
                    Random.Range(leftBound, rightBound), Random.Range(0, IGrid.Ins.width), Team.Enemy, 1);
                e.BecomeInActive();
                demoEnemies.Add(e.GetComponent<Entity>());
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