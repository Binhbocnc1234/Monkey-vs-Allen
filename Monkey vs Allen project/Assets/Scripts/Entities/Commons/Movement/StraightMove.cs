using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class StraightMove : Move
{
    public override void UpdateBehaviour() {
        base.UpdateBehaviour();
        e.model.transform.Translate(new Vector2(e.GetRealMoveSpeed() * Time.deltaTime * GetNormalizedDirection(), 0));
    }
}
