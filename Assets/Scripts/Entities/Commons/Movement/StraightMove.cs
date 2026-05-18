using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class StraightMove : Move
{
    public override void UpdateBehaviour(float deltaTime) {
        e.model.SetPosition(e.model.GetPosition() + new Vector2(GetUnityMoveSpeed(e) * deltaTime * GetNormalizedDirection(), 0));
    }
}
