using UnityEngine;

internal class AlienResourceManager : Singleton<AlienResourceManager> {
    public Timer resourceTimer;
    public int costToUpgrade = 5;
    public int increaseAfterUpgrade = 10;
    public int upgradeCnt = 0;
    [ReadOnly] public int currentEnemyResource = 0;
    // private float difficultyToFactor; // difficulty càng cao thì factor thấp, khiến thời gian delay để sinh resource càng thấp
    protected override void Awake() {
        base.Awake();

    }
    public void Initialize() {
        // difficultyToFactor = 1 - (BattleInfo.levelSO.difficulty - 1) / 4;
    }
    void Update() {
        if(BattleInfo.gameState != GameState.Fighting) { return; }
        currentEnemyResource = BattleInfo.teamDict[Team.Right].resource;
        if(upgradeCnt > 0 && resourceTimer.Count()) {
            BattleInfo.teamDict[Team.Right].resource += BattleInfo.resourcePerGeneration;
        }
    }
    public bool CanUpgrade() {
        return BattleInfo.teamDict[Team.Right].resource >= costToUpgrade;
    }
    public void Upgrade() {
        BattleInfo.teamDict[Team.Right].resource -= costToUpgrade;
        upgradeCnt++;
        if(upgradeCnt == 1) {
            resourceTimer = new Timer(BattleInfo.resourceDelay , true);
        }
        else {
            resourceTimer.totalTime = (float)BattleInfo.resourceDelay / upgradeCnt ;
        }
        costToUpgrade += increaseAfterUpgrade;
    }
}