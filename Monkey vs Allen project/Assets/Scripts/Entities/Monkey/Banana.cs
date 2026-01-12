using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana: DropBodyPart{
    [ReadOnly] public bool isDisappearing = false;
    int count;
    void Awake() {
        canFadeOut = false;
    }
    public void SetBananaCount(int count) {
        this.count = count;
    }
    protected override void Update() {
        base.Update();
        if(state == DropPartState.FadeOut) {
            transform.Translate(new Vector2(0, Time.deltaTime * 5));
        }
    }
    void OnMouseUp() {
        state = DropPartState.FadeOut;
        BattleInfo.ChangeBananaCnt(count);
    }

}