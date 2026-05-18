using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehindAlliesMove : StraightMove
{
    public float stopBehindAllyRange = 1;
    public override bool CanActive() {
        if (base.CanActive() == false) { // Bị kẻ địch cản lại
            return false;
        }
        foreach(IEntity otherE in EContainer.Ins.GetEntitiesByLane(e.lane)) {
            if(otherE == e || otherE.team != e.team) continue;
            if(otherE.DistanceToBase() - e.DistanceToBase() >= stopBehindAllyRange) { // Có đồng đội phía trước dẫn đường
                return true;
            }
        }
        return false;
    }
}
