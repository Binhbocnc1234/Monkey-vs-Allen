using UnityEngine;

public class StraightBullet : CollisionBullet {
    private Vector3 direction;
    private float disappearRange;
    private float startX;
    private bool isFading = false;
    public new void Initialize(float damage, IEntity owner) {
        base.Initialize(damage, owner);
        this.disappearRange = owner[ST.Range];
        startX = this.transform.position.x;
        if(owner.team == Team.Player) {
            direction = Vector3.right;
        }
        else {
            direction = Vector3.left;
        }
    }
    protected override void Update() {
        if(isFading) {
            // bool isCompletelyFadeOut = true;
            // foreach(SpriteRenderer renderer in model.sprites) {
            //     Color c = renderer.color;
            //     c.a -= Time.deltaTime * speedMultiplier;
            //     if(c.a > 0) {
            //         isCompletelyFadeOut = false;
            //     }
            // }
        }
        else {
            base.Update();
            transform.position += direction * speed * Time.deltaTime;
            if(Mathf.Abs(transform.position.x - startX) >= disappearRange * IGrid.CELL_SIZE - 0.5f) {
                isFading = true;
            }
        }
        
    }
    
}