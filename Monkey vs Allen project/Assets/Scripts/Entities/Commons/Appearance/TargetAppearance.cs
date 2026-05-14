using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetAppearance : EntityAppearance
{
    public SpriteRenderer sign;
    public List<Sprite> targetSprites;
    public override void Initialize() {
        base.Initialize();
        e.OnHealthChanged += UpdateSign;
    }
    void UpdateSign(float healthAmount){
        int index = (int)((1 - e.GetHealthPercentage() - 0.01f) / 0.25f);
        sign.sprite = targetSprites[index];
    }
}
