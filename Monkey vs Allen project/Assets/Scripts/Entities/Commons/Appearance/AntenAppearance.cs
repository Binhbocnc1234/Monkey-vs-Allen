using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntenAppearance : MonoBehaviour {
    IEntity e;
    public SpriteRenderer anten1, anten2;
    public Sprite S_anten;
    private Sprite old_anten;
    protected void Awake(){
        e = GetComponent<IEntity>();
        old_anten = anten1.sprite;
        e.OnHealthChanged += (diff) => {
            if(e.GetHealthPercentage() <= 0.5f) {
                anten1.sprite = S_anten;
            }
            else {
                anten1.sprite = old_anten;
            }
        };
    }
}