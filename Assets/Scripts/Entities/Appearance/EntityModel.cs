using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Linq;

/*
    Why in general structure of Entity, there is seperate gameObject called Model?
    Effect like airbone can adjust Entity's appearance position without changing its logic position 
*/
/// <summary>
/// Position of model is adjusted so that it align well with grid system. Do not change model's position at runtime <br/>
/// </summary>
public class EntityModel : Model {
    public IEntity e { get; private set; }
    public Transform firePoint;
    protected override void Awake() {
        base.Awake();
        sortingGroup.sortingLayerName = "Entities";
    }
    public void AssignEntity(IEntity e) {
        this.e = e;
        this.sortingGroup.sortingOrder = 1 - e.lane;
        foreach(EntityAppearance appearance in GetComponents<EntityAppearance>()) {
            appearance.Initialize(this);
        }
        if(firePoint != null && e.GetBehaviour<RangedAttack>() != null) {
            e.GetBehaviour<RangedAttack>().firePoint = firePoint;
        }
        
    }
}