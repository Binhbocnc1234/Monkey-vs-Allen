using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaDamage {
    public enum Range{Low, Medium, High}
    public static void CreateAreaDamage(int lane, float x, float damage, Range range, IEntity attacker) {
        List<IEntity> entities = new();
        if(range == Range.Medium) {
            entities.AddRange(IEntityRegistry.Ins.GetEntitiesByLane(lane));
            entities.AddRange(IEntityRegistry.Ins.GetEntitiesByLane(lane + 1));
            entities.AddRange(IEntityRegistry.Ins.GetEntitiesByLane(lane - 1));
        }
        else if(range == Range.High) {
            entities.AddRange(IEntityRegistry.Ins.GetEntitiesByLane(lane));
            entities.AddRange(IEntityRegistry.Ins.GetEntitiesByLane(lane + 1));
            entities.AddRange(IEntityRegistry.Ins.GetEntitiesByLane(lane - 1));
            entities.AddRange(IEntityRegistry.Ins.GetEntitiesByLane(lane + 2));
            entities.AddRange(IEntityRegistry.Ins.GetEntitiesByLane(lane - 2));
        }
        float maxDiffX = range switch {
            Range.Medium => 1.5f,
            Range.High => 2.5f,
            Range.Low => 0.5f,
            _ => 0
        };
        foreach(IEntity otherE in IEntityRegistry.Ins.GetEntities()) {
            if (Mathf.Abs(x - otherE.gridPos.x) <= maxDiffX && otherE.team == attacker.team) {
                otherE.TakeDamage(new DamageContext(damage, attacker, otherE, false));
            }
        }
    }
}