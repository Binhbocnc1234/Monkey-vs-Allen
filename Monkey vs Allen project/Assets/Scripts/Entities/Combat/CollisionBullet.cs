using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class CollisionBullet : Bullet {
    // public float distanceToTarget, dx, dy;
    protected virtual void Update() {
        foreach(IEntity e in EContainer.Ins.GetEntitiesByLane(this.lane)) {
            if(e.team == this.team) continue;
            // distanceToTarget = GetDiffToBounds(transform.position, e.model.boxCollider.bounds);
            if(IsCollidedWith(transform.position, e.model.GetBound())) {
                OnHit(e);
                break;
            }
        }
    }
    protected bool IsCollidedWith(Vector2 pos, Bounds bounds) {
        return GetDiffToBounds(pos, bounds) <= (0.02f * speed);
    }
    protected float GetDiffToBounds(Vector2 pos, Bounds bounds) {
        float dx = 0f;
        if(pos.x < bounds.min.x) {
            dx = bounds.min.x - pos.x;
        }
        else if(pos.x > bounds.max.x) {
            dx = pos.x - bounds.max.x;
        }

        float dy = 0f;
        if(pos.y < bounds.min.y) {
            dy = bounds.min.y - pos.y;
        }
        else if(pos.y > bounds.max.y) {
            dy = pos.y - bounds.max.y;
        }
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}
