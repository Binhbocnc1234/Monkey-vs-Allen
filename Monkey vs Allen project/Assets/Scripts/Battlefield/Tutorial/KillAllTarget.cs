using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAllTarget : Tutorial
{
    public Arrow arrowPrefab;
    public List<Arrow> arrows;
    public override void Initialize() {
        base.Initialize();
        StartTutorial();
    }
    public override void StartTutorial() {
        base.StartTutorial();
        GridCamera.Ins.OnFinishedMoving += ShowArrow;
    }
    void ShowArrow(){
        foreach(Entity target in EContainer.Ins.GetTargetEnemy()){
            Arrow newArrow = Instantiate(arrowPrefab, target.GetWorldPosition() + new Vector2(0, 1), Quaternion.identity, this.transform);
            newArrow.pointingDirection = Direction.Down;
            arrows.Add(newArrow);
        }
        GridCamera.Ins.OnFinishedMoving -= ShowArrow;
        StartCoroutine(CountdownFinished(4));
    }

    IEnumerator CountdownFinished(float duration){
        Timer timer = new Timer(duration, true);
        while(true){
            if (timer.Count()){
                break;
            }
            else{
                yield return null;
            }
        }
        CompleteTutorial();
    }
    public override void CompleteTutorial() {
        base.CompleteTutorial();
        foreach(Arrow arrow in arrows){
            Destroy(arrow.gameObject);
        }
    }

}
