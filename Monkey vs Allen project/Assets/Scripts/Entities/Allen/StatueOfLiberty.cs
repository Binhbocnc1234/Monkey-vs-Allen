using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueOfLiberty : Entity
{
    IEffect effect;
    public override void Initialize(EntitySO so, Team team) {
        base.Initialize(so, team);
        OnEntityDeath += OnKilled;
        GameEvents.AdjustCostChangeGlobally(2);
    }

    void OnKilled(Entity e){

    }
}
