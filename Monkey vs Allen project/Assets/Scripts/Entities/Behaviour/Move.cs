using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Move : IBehaviour
{
    public float moveSpeed;
    [ReadOnly] public bool moveToTheRight = true;
    protected virtual void Update(){
        if (moveToTheRight){
            transform.Translate(new Vector2(moveSpeed * Time.deltaTime, 0));
        }
        else{
            transform.Translate(new Vector2(-moveSpeed * Time.deltaTime, 0));
        }
    }
    public override void Initialize(){
        base.Initialize();
        e = GetComponent<Entity>();
        var so = e.so;
        moveSpeed = so.moveSpeed;
        moveToTheRight = so.team == Team.Player;
        dangerPoint = 0;
    }
    public override void OnStateChange(EntityState state) {
        if (state == EntityState.Attacking){ //Entity cannot move when attacking
            this.enabled = false;
        }
        else{
            this.enabled = true;
        }
    }
}
