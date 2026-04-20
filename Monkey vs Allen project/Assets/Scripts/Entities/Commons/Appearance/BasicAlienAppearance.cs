using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class BasicAlienAppearance : MonoBehaviour
{
    public DropBodyPart arm;
    public SpriteRenderer reaction;
    public Sprite S_reaction, S_anten, S_attack;
    public bool enableHandDrop = false;
    private Sprite old_reaction;
    private bool isFirstTimeHalfHealth = false;
    Entity e;
    protected void Awake(){
        e = GetComponent<Entity>();
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
