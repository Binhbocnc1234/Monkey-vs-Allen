using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class BasicAllenAppearance : IBehaviour
{
    public DropBodyPart arm;
    public SpriteRenderer reaction, anten1, anten2;
    public Sprite S_reaction, S_anten, S_attack;
    private Sprite old_reaction, old_anten;
    private bool isFirstTimeHalfHealth = false;
    public override void Initialize(){
        base.Initialize();
        e = GetComponent<Entity>();
        old_reaction = reaction.sprite;
        old_anten = anten1.sprite;
    }
    protected override void UpdateBehaviour(){
        if (e.state == EntityState.Attacking){
            reaction.sprite = S_attack;
        }
        else if (e.health <= e.maxHealth/2){
            if (isFirstTimeHalfHealth == false){
                reaction.sprite = S_reaction;
                anten1.sprite = S_anten;
                isFirstTimeHalfHealth = true;
                arm.Initialize(e.laneIndex, 3);
            }
        }
        else{
            reaction.sprite = old_reaction;
            anten1.sprite = old_anten;
        }
    }

}
