using System;
using UnityEngine;
using TMPro;

public class CollectBananaTutorial : Tutorial
{
    private BananaTree tree;
    void Awake() {
        foreach (IEntity e in EContainer.Ins.GetEntitiesByLane(2))
        {
            tree = e.GetBehaviour<BananaTree>();
            if(tree != null) {
                tree.isEnable = false;
                e.ReturnToIdleBehaviour();
                break;
            }
        }
    }
    public override void Initialize() {
        base.Initialize();
        if(tree == null) return;
        tree.isEnable = true;
        tree.cooldownTimer.SetCurTime(tree.cooldownTimer.totalTime * 0.1f);
        tree.OnStateChanged += (state) => {
            if (this == null || state == BananaTree.State.Growing) return;
            StartTutorial();
        };
    }
    public override void StartTutorial() {
        base.StartTutorial();
        GridCamera.Ins.MoveTowardPlayerHouse();
        BattleInfo.teamDict[Team.Left].OnResourceChange += CompleteTutorial;
    }
    public override void CompleteTutorial() {
        base.CompleteTutorial();
        BattleInfo.teamDict[Team.Left].OnResourceChange -= CompleteTutorial;
    }
}