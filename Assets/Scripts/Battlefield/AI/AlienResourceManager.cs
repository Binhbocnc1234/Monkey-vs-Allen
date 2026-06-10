using UnityEngine;

public class AlienResourceManager {
    private static AlienResourceManager _instance;
    public static AlienResourceManager Ins {
        get {
            if (_instance == null) {
                _instance = new AlienResourceManager();
            }
            return _instance;
        }
    }
    public static void ResetInstance() {
        _instance = null;
    }

    public Timer resourceTimer;
    public int costToUpgrade = 5;
    public int increaseAfterUpgrade = 10;
    public int upgradeCnt = 0;
    public int currentEnemyResource = 0;
    
    public void Initialize() {
    }
    public void Update(float deltaTime) {
        if(BattleInfo.gameState != GameState.Fighting) { return; }
        currentEnemyResource = BattleInfo.teamDict[Team.Right].resource;
        if(upgradeCnt > 0 && resourceTimer.Count(deltaTime)) {
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