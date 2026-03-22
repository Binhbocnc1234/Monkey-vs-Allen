using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class BasicAllenAppearance : MonoBehaviour
{
    public DropBodyPart arm;
    public SpriteRenderer reaction, anten1, anten2;
    public Sprite S_reaction, S_anten, S_attack;
    public bool enableHandDrop = false;
    private Sprite old_reaction, old_anten;
    private bool isFirstTimeHalfHealth = false;
    Entity e;
    protected void Awake(){
        e = GetComponent<Entity>();
        old_reaction = reaction.sprite;
        old_anten = anten1.sprite;
    }
    void Update(){
        if (e.GetActiveBehaviour() is Attack){
            reaction.sprite = S_attack;
        }
        else if (e[ST.Health] <= e[ST.MaxHealth]/2){
            if (isFirstTimeHalfHealth == false){
                reaction.sprite = S_reaction;
                anten1.sprite = S_anten;
                isFirstTimeHalfHealth = true;
                if(enableHandDrop) {
                    arm.Initialize(e.lane, 3);
                }
            }
        }
        else{
            reaction.sprite = old_reaction;
            anten1.sprite = old_anten;
        }
    }

}
