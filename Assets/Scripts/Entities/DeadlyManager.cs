using UnityEngine;
using System.Collections.Generic;

public class DeadlyManager {
    private Dictionary<IEntity, int> markCount = new();
    public void Mark(IEntity entity) {
        markCount[entity] += 1;
        if (markCount[entity] >= 5) {
            entity.Die();
        }
    }
}