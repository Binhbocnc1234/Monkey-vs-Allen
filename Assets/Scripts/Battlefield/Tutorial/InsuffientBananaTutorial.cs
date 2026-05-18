using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InsuffientBananaTutorial : Tutorial
{
    private enum State{
        ZoomUp,
        Waiting,
        ZoomDown,
    }
    [ReadOnly] private State state;
    private float oldSize = 0;
    private TMP_Text bananaText;
    private Timer waitingTimer = new Timer(3, true);
    public override void Initialize() {
        base.Initialize();
        oldSize = BananaCounterUI.Ins.tmp.fontSize;
        bananaText = BananaCounterUI.Ins.tmp;
        state = State.ZoomUp;
    }
    void Update(){
        if (state == State.ZoomUp){
            bananaText.fontSize += 50 * Time.deltaTime;
            if (bananaText.fontSize > 100){
                state = State.Waiting;
            }
        }
        else if (state == State.Waiting){
            if (waitingTimer.Count()){
                state = State.ZoomDown;
            }
        }
        else if (state == State.ZoomDown){
            bananaText.fontSize -= 50 * Time.deltaTime;
            if (bananaText.fontSize <= oldSize){
                bananaText.fontSize = oldSize;
                CompleteTutorial();
            }
        }
    }
}
