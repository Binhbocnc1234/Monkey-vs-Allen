using UnityEngine;

public class CollisionBullet : Bullet
{
    public new void Initialize(float speed, int damage, Entity owner) {
        base.Initialize(speed, damage, owner);
        if(owner.team == Team.Player) {
            direction = Vector3.right;
        }
        else {
            direction = Vector3.left;
        }
    }
    protected virtual void Update()
    {
        // Di chuyển theo đường thẳng
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra va chạm với Enemy
        Entity target = other.GetComponent<Entity>();
        if (target != null)
        {
            OnHit(target);
        }
    }
}
