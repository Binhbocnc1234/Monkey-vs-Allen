using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana: DropBodyPart{
    [ReadOnly] public bool isDisappearing = false;
    public MonoBehaviour blinkEffect; 
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
        if (Input.GetMouseButtonUp(0))
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            Camera cam = Camera.main;
            if(boxCollider == null || cam == null) return;

            Vector3 clickPos = cam.ScreenToWorldPoint(Input.mousePosition);
            clickPos.z = transform.position.z;

            if(boxCollider.bounds.Contains(clickPos)) {
                BananaOnClick();
            }
        }
    }
    void BananaOnClick() {
        state = DropPartState.FadeOut;
        Destroy(blinkEffect);
        BattleInfo.teamDict[Team.Left].resource += count;
    }

}