using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class CollisionBullet : Bullet {
    protected virtual void Update() {
        foreach(IEntity e in EContainer.Ins.GetEntitiesByLane(this.lane)) {
            if(e.team == this.team) continue;
            if(IsCollidedWith(transform.position, e.model.boxCollider.bounds)) {
                OnHit(e);
            }
        }
    }
    protected bool IsCollidedWith(Vector3 pos, Bounds bounds) {
        return GetDiffToBounds(pos, bounds) <= (0.1f * speed);
    }
    protected float GetDiffToBounds(Vector3 pos, Bounds bounds) {
        if(bounds.Contains(pos)) {
            return 0;
        }
        else {
            float[] dist = new float[4]{
                Mathf.Abs(pos.x - bounds.max.x),
                Mathf.Abs(pos.x - bounds.min.x),
                Mathf.Abs(pos.y - bounds.max.y),
                Mathf.Abs(pos.x - bounds.min.x)
            };
            return dist.Min();
        }
    }
}
