using UnityEngine;
using System.Collections.Generic;

public class ReinforcementModifier : MonoBehaviour {
    public UDictionary<EntitySO, int> army;
    public int emergencyCnt = 2;
    public int armyLevel = 1;
    public bool isCalled = false;
    public int activeAfterTargetNumber;
    void Awake() {
        if(IEntityRegistry.Ins.GetTargetCount(Team.Right) >= 3) {
            activeAfterTargetNumber = Random.Range(1, 4);
        }
        else {
            activeAfterTargetNumber = 1;
        }
    }
    
    void Update() {
        if(!isCalled && IEntityRegistry.Ins.GetTargetCount(Team.Right) == activeAfterTargetNumber) {
            foreach(IEntity e in IEntityRegistry.Ins.GetEntities()) {
                bool con1 = e.GetSO().IsContainTribes(new List<Tribe>() { Tribe.Target });
                float con2 = e.GetHealthPercentage();
                if(e.team == Team.Right && e.GetSO().IsContainTribes(new List<Tribe>() { Tribe.Target }) && e.GetHealthPercentage() <= 0.5f) {
                    isCalled = true;
                    SpawnReinforcement(e.lane);
                    e.GetEffectable().ApplyEffect(new GeneralStatEffect(new() {
                        new StatModifier(Operator.Addition, ST.Armor, 40),
                        new StatModifier(Operator.Addition, ST.MagicResistance, 40)
                    }, duration: 5));
                }

            }
        }
    }
    void SpawnReinforcement(int emergencyLane) {
        int spawnedInEmergencyLane = 0;
        List<int> openLanes = new();
        for(int i = 0; i < IGrid.Ins.openLanes.Length; ++i) {
            if(IGrid.Ins.openLanes[i]) {
                openLanes.Add(i);
            }
        }
        foreach(var pair in army) {
            for(int i = 0; i < pair.Value; ++i) {
                EntitySetting setting = new EntitySetting {
                    lane = Utilities.RandomElement(openLanes),
                    x = IGrid.Ins.width,
                    level = armyLevel,
                    so = pair.Key,
                    team = Team.Right
                };
                if(spawnedInEmergencyLane < emergencyCnt) {
                    spawnedInEmergencyLane++;
                    setting.lane = emergencyLane;
                }
                IEntityRegistry.Ins.CreateEntity(setting);
            }
        }
    }
}