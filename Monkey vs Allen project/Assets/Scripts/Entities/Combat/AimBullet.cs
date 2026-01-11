using UnityEngine;

public class AimBullet : Bullet
{
    [Header("Aim Settings")]
    public float hitDistance = 0.5f;
    public float turnSpeed = 5f;
    private Entity target;
    private Vector3 targetPosition;

    public void Initialize(float speed, int damage, Entity owner, Entity target)
    {
        base.Initialize(speed, damage, owner);
        targetPosition = target.transform.position;
    }

    void Update()
    {
        if (target != null && !target.IsDead())
        {
            // Cập nhật vị trí mục tiêu
            targetPosition = ((MonoBehaviour)target).transform.position;
            
            // Bám theo mục tiêu
            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            direction = Vector3.Lerp(direction, targetDirection, turnSpeed * Time.deltaTime);
            
            // Di chuyển về phía mục tiêu
            transform.position += direction * speed * Time.deltaTime;
            
            // Kiểm tra khoảng cách để va chạm
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance <= hitDistance)
            {
                OnHit(target);
            }
        }
        else
        {
            // Nếu không có mục tiêu hoặc target chết, vẫn di chuyển về địa điểm target chết
            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            direction = Vector3.Lerp(direction, targetDirection, turnSpeed * Time.deltaTime);
            
            // Di chuyển về địa điểm target chết
            transform.position += direction * speed * Time.deltaTime;
            
            // Kiểm tra khoảng cách để tự hủy
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance <= hitDistance)
            {
                DestroyBullet();
            }
        }
    }
}
