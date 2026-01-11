using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class CollectBananaTutorial : Tutorial
{
    public override void Initialize() {
        base.Initialize();
        StartTutorial();
        foreach(Entity e in EContainer.Ins.GetEntitiesByLane(2)) {
            BananaTree tree = e.GetComponent<BananaTree>();
            if(tree != null) {
                tree.SetBehaviourEnable(true);
                tree.cooldownTimer.SetCurTime(tree.cooldown * 0.85f);
                tree.OnBananaGenerated += (info) => {
                    if(this == null) return;
                    StartTutorial();
                };
                break;
            }
        }
    }
    void Update() {
        
    }
    public override void StartTutorial() {
        base.StartTutorial();
        GridCamera.Ins.MoveTowardPlayerHouse();
        BattleInfo.OnBananaChange += CompleteTutorial;
    }
    public override void CompleteTutorial() {
        base.CompleteTutorial();
        BattleInfo.OnBananaChange -= CompleteTutorial;
        PauseManager.Ins.DePause();
    }

}
