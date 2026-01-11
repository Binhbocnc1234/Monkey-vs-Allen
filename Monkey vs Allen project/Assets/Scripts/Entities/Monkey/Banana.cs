using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana: DropBodyPart{
    [ReadOnly] public bool isDisappearing = false;
    private bool isReceivedBanana = false;
    void Start(){
        
    }
    protected override void Update(){
        base.Update();
        if (state == DropPartState.FadeOut){
            if (isReceivedBanana == false){
                BattleInfo.ChangeBananaCnt(1);
                isReceivedBanana = true;
            }
            transform.Translate(new Vector2(0, Time.deltaTime*5));
        }
    }
    // void OnMouseDown(){
    //     //Collect banana
    //     BattleInfo.ChangeBananaCnt(1);
    //     state = DropPartState.FadeOut;
    // }

}