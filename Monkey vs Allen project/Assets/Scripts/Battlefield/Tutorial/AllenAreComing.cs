using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllenAreComing : Tutorial
{
    public EntitySO basicAllenSO;
    public float duration;
    private IEntity demoEntity;
    public override void Initialize() {
        base.Initialize();
        StartTutorial();
    }
    public override void StartTutorial() {
        base.StartTutorial();
        StartCoroutine(CountdownFinished(duration));
    }

    IEnumerator CountdownFinished(float duration){
        IGrid grid = GridSystem.Ins;
        GridCamera.Ins.MoveTowardEnemyHouse();
        demoEntity = IEntityRegistry.Ins.CreateEntity(basicAllenSO, grid.width - 2, 2, Team.Enemy);
        yield return new WaitWhile(() => GridCamera.Ins.isMoving == true);
        Timer timer = new Timer(duration, true);
        while(true){
            if (timer.Count()){
                break;
            }
            else{
                yield return null;
            }
        }
        GridCamera.Ins.MoveTowardPlayerHouse();
        yield return new WaitWhile(() => GridCamera.Ins.isMoving);
        CompleteTutorial();
    }
}
