using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAlienAppearance : EntityAppearance
{
    public DropBodyPart arm;
    public SpriteRenderer reaction;
    public Sprite S_reaction, S_anten, S_attack;
    public bool enableHandDrop = false;
    private Sprite old_reaction;
    private bool isFirstTimeHalfHealth = false;
    void Awake(){
        old_reaction = reaction.sprite;
    }
    void Update(){
        if (e.GetActiveBehaviour() is Attack){
            reaction.sprite = S_attack;
        }
        else if (e.GetHealthPercentage() <= 0.5f){
            if (isFirstTimeHalfHealth == false){
                reaction.sprite = S_reaction;
                isFirstTimeHalfHealth = true;
                if(enableHandDrop) {
                    arm.Initialize(e.lane, 3);
                }
            }
        }
        else{
            reaction.sprite = old_reaction;
        }
    }

}
