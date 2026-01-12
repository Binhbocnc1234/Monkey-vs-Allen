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
            if (tree != null) {
                break;
            }
        }
    }
    public override void Initialize() {
        base.Initialize();
        tree.SetBehaviourEnable(true);
        tree.cooldownTimer.SetCurTime(tree.cooldownTimer.totalTime * 0.85f);
        tree.OnStateChanged += (state) => {
            if (this == null || state == BananaTree.State.Growing) return;
            StartTutorial();
        };
    }
    public override void StartTutorial() {
        base.StartTutorial();
        GridCamera.Ins.MoveTowardPlayerHouse();
        BattleInfo.OnBananaChange += CompleteTutorial;
    }
    public override void CompleteTutorial() {
        base.CompleteTutorial();
        BattleInfo.OnBananaChange -= CompleteTutorial;
    }

}
