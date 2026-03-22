using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class CollectBananaTutorial : Tutorial
{
    private BananaTree tree;
    void Awake() {
        foreach (Entity e in EContainer.Ins.GetEntitiesByLane(2))
        {
            tree = e.GetComponent<BananaTree>();
            if(tree != null) {
                tree.enabled = false;
                e.ReturnToIdleBehaviour();
                break;
            }
        }
    }
    public override void Initialize() {
        base.Initialize();
        tree.enabled = true;
        tree.cooldownTimer.SetCurTime(tree.cooldownTimer.totalTime * 0.1f);
        tree.OnStateChanged += (state) => {
            if (this == null || state == BananaTree.State.Growing) return;
            StartTutorial();
        };
    }
    public override void StartTutorial() {
        base.StartTutorial();
        GridCamera.Ins.MoveTowardPlayerHouse();
        BattleInfo.teamDict[Team.Player].OnResourceChange += CompleteTutorial;
    }
    public override void CompleteTutorial() {
        base.CompleteTutorial();
        BattleInfo.teamDict[Team.Player].OnResourceChange -= CompleteTutorial;
    }

}
