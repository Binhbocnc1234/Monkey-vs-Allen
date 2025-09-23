using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Target : IBehaviour
{

    public SpriteRenderer sign;
    public List<Sprite> sprites;
    public override void Initialize() {
        base.Initialize();
        e.OnHealthChanged += UpdateSign;
    }
    void UpdateSign(int healthAmount){
        sign.sprite = sprites[(int)((e.GetHealthPercentage() - 0.01f) / 0.25f)];
    }
}
