using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyManager: Singleton<EnemyManager>{
    private IGrid grid;
    public MeterorUI meteor;
    [ReadOnly] public int meteorIndex = 0;
    public float meteoriteUpgradeDelay = 100, meteoriteUpgradeDiff = 60;
    private Timer meteoriteTimer;
    private float coolDownFactor = 1f;
    private List<Entity> demoEnemies = new List<Entity>();
    void Start(){
        meteoriteTimer = new Timer(meteoriteUpgradeDelay);
        grid = GridSystem.Instance;
        foreach(CardSO so in BattleManager.Instance.levelInfo.enemies){
            Card newCard = new Card();
            newCard.ApplyCardSO(so);
            BattleInfo.choosenEnemies.Add(newCard);
        }
        enabled = false;
    }
    void Update(){
        if (meteoriteTimer.Count()){
            meteorIndex++;
            // healthFactor += 0.3f;
            coolDownFactor -= 0.25f;
            foreach(Card card in BattleInfo.choosenEnemies){
                card.SetCoolDown(card.cooldown * coolDownFactor);
            }
            meteor.ChangeState(meteorIndex);
        }
        for(int i = 0; i < BattleInfo.choosenEnemies.Count; ++i){
            ICard card = BattleInfo.choosenEnemies[i];
            card.cooldownTimer.Count(false);
            if(card.cooldownTimer.isEnd) {
                card.cooldownTimer.Reset();
                CreateEnemy(card.so.prefab.GetComponent<Entity>(), PickLane(card));
            }
        }
    }
    int PickLane(ICard so){
        float[] distances = new float[grid.height];
        int[] monkeysValue = new int[grid.height];
        int res = Random.Range(0, grid.height);
        float maxDanger = 10;
        for(int i = 0; i < grid.height; ++i){
            bool haveMonkey = false;
            distances[i] = 999f;
            List<Entity> entities = EContainer.GetEntitiesByLane(i);
            foreach(Entity e in entities){
                if (e.team == Team.Player && e.GetComponent<Target>() == null){
                    // Debug.Log(e.name);
                    float dis = e.Distance(Entity.targetEnemies[i]);
                    if (e.entityType == EntityType.Monkey){
                        monkeysValue[i] += e.health/6;
                        foreach(IBehaviour behaviour in e.behaviours){
                            monkeysValue[i] += behaviour.dangerPoint;
                        }
                        haveMonkey = true;
                    }
                    distances[i] = Mathf.Min(distances[i], dis) / grid.cellSize;
                }
            }
            if (haveMonkey){
                float danger = monkeysValue[i] + Mathf.Max(grid.width - distances[i] - 2, 0) ;
                if (danger > maxDanger){
                    
                    maxDanger = danger;
                    res = i;
                }
            }
        }
        return res;
    }
    void CreateEnemy(Entity prefab, int lane){

        Entity newEnemy = grid.GetCell(grid.width-1, lane).PlaceObject(prefab.gameObject).GetComponent<Entity>();
        // int newHealth = (int)Mathf.Round(newEnemy.health * healthFactor);
        newEnemy.Initialize();
        
        // newEnemy.health = newHealth;
        // newEnemy.maxHealth = newHealth;
        // Debug.Log($"Enemy health: {newHealth}");
        newEnemy.SetEntityState(EntityState.Walk);
        
    }
    public void ShowEnemy(){
        float upperBound = grid.GridToWorldPosition(grid.width - 1, grid.height - 1).y;
        float lowerBound = grid.GridToWorldPosition(grid.width - 1, 0).y;
        float leftBound = grid.GridToWorldPosition(grid.width, 0).x;
        float rightBound = leftBound + GridSystem.Instance.cellSize*2;
        Vector2 RandomPosInBound(){
            return new Vector2(Random.Range(leftBound, rightBound), Random.Range(lowerBound, upperBound));
        }
        // const float minDemoEnem
        foreach(Card card in BattleInfo.choosenEnemies){
            for(int i = 0; i < 2; ++i){
                GameObject newEnemy = Instantiate(card.so.prefab, RandomPosInBound(), Quaternion.identity);
                Entity e = newEnemy.GetComponent<Entity>();
                e.SetEntityState(EntityState.Idle);
                demoEnemies.Add(newEnemy.GetComponent<Entity>());
            }
        }
    }
    public void ClearDemoEnemy(){
        foreach(Entity e in demoEnemies){
            Destroy(e.gameObject);

        }
        demoEnemies.Clear();
    }
}