using UnityEngine;

public class StraightBullet : CollisionBullet {
    private Vector3 direction;
    private float disappearRange;
    private float startX;
    public bool isFading = false;

    public override void Initialize(BulletSpawnRequest request) {
        base.Initialize(request);
        disappearRange = request.owner[ST.Range] * 1.2f;
        startX = request.position.x;
        direction = request.owner.team == Team.Left ? Vector3.right : Vector3.left;
    }

    protected override void Update() {
        if(isFading) {
        }
        else {
            base.Update();
            transform.position += direction * speed * Time.deltaTime;
            if(Mathf.Abs(transform.position.x - startX) >= disappearRange * IGrid.CELL_SIZE) {
                isFading = true;
                Destroy(this.gameObject);
            }
        }
        
    }
    
}
