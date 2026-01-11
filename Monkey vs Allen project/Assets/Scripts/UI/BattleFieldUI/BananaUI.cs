using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaUI : MonoBehaviour
{
    // private enum State{
    //     Waiting,
    //     Moving,
    // }
    // private State state = State.Waiting;
    private BananaCounterUI counterUI;
    private RectTransform rect;
    private Timer waitingTimer = new Timer(0.5f, false), moveTimer = new Timer(0.5f, false);

    void Start()
    {
        rect = GetComponent<RectTransform>();
        counterUI = BananaCounterUI.Ins;
    }
    void Update(){
        if (waitingTimer.Count()){
            if (moveTimer.Count() == false){
                rect.anchoredPosition = Vector3.Lerp(
                    rect.anchoredPosition, 
                    counterUI.GetComponent<RectTransform>().anchoredPosition,
                    moveTimer.GetPercent()
                );
            }
            else{
                BattleInfo.ChangeBananaCnt(1);
                Destroy(this.gameObject);
            }
        }
    }

}
