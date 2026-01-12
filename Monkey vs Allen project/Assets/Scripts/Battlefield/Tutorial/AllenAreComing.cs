using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllenAreComing : Tutorial
{
    public EntitySO basicAllenSO;
    public float duration;
    private Entity demoEntity;
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
        demoEntity = EContainer.Ins.CreateEntity(basicAllenSO, new Vector2Int(grid.width - 2, 2), Team.Enemy);
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
    public override void CompleteTutorial() {
        base.CompleteTutorial();
    }

}
