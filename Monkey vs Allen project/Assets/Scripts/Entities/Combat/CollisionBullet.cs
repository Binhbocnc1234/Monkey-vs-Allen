using UnityEngine;

public class CollisionBullet : Bullet
{
    void Update()
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
