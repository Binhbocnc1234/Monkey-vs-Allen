using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity), typeof(Rotater))]
public class Move : IBehaviour
{
    [ReadOnly] public float moveSpeed;
    [ReadOnly] public bool moveToTheRight = true;
    public override void Initialize() {
        base.Initialize();
        var so = e.so;
        moveSpeed = so.moveSpeed;
        dangerPoint = 0;
        e.defaultState = EntityState.Walk;
    }
    protected override void UpdateBehaviour() {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime * GetNormalizedDirection(), Space.World);
    }
    protected int GetNormalizedDirection() {
        return e.team == Team.Player ? 1 : -1;
    }
    public override void OnStateChange(EntityState state) {
        if (state != EntityState.Walk){ //Entity cannot move when attacking
            SetBehaviourEnable(false);
        }
        else if (state == EntityState.Walk){ // Ready to walk
            SetBehaviourEnable(true);
        }
    }
}
