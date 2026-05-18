using UnityEngine;

public class LobBullet : CollisionBullet{
    private float upwardVec;
    public void Initialize(float damage, IEntity owner, float upwardVec) {
        base.Initialize(damage, owner);
        this.upwardVec = upwardVec;
    }
    protected override void Update() {
        base.Update();
        transform.position += Vector3.up * upwardVec * Time.deltaTime;
        upwardVec -= TechnicalInfo.gravity * Time.deltaTime;
    }
}