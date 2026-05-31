using UnityEngine;

public class LobBullet : CollisionBullet{
    private float upwardVec;

    public override void Initialize(BulletSpawnRequest request) {
        base.Initialize(request);
        if (request.target != null && request.owner != null) {
            upwardVec = CalculateUpwardVec(
                request.position,
                request.target,
                speed,
                request.owner[ST.MoveSpeed]
            );
        }
    }

    static float CalculateUpwardVec(Vector2 start, IEntity target, float bulletVecX, float targetVecX) {
        float dx = target.gridPos.x - start.x;
        float dv = bulletVecX - targetVecX;
        if(dv == 0) return float.NaN;

        float t = dx / dv;
        if(t <= 0) return float.NaN;

        const float g = 10f;
        float dy = target.gridPos.y - start.y;

        return (dy / t) + 0.5f * g * t;
    }

    protected override void Update() {
        base.Update();
        transform.position += Vector3.up * upwardVec * Time.deltaTime;
        upwardVec -= TechnicalInfo.gravity * Time.deltaTime;
    }
}
