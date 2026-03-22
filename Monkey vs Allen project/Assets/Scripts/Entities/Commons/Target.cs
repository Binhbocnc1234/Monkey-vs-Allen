using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Target : MonoBehaviour
{
    public SpriteRenderer sign;
    public List<Sprite> targetSprites;
    Entity e;
    protected virtual void Awake() {
        e = GetComponent<Entity>();
        e.OnHealthChanged += UpdateSign;
    }
    void UpdateSign(float healthAmount){
        int index = (int)((1 - e.GetHealthPercentage() - 0.01f) / 0.25f);
        sign.sprite = targetSprites[index];
    }
}
