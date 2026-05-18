using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAllTarget : Tutorial
{
    Timer timer;
    public override void Initialize() {
        base.Initialize();
        StartTutorial();
    }
    public override void StartTutorial() {
        base.StartTutorial();
        timer = new Timer(4, true);
        GridCamera.Ins.MoveTowardEnemyHouse();
    }
    void Update() {
        if(timer.Count()) {
            CompleteTutorial();
        }
    }
}
