using UnityEngine;

public class AimBullet : Bullet
{
    public float hitDistance = 0.5f;
    public float turnSpeed = 5f;
    private IEntity target;
    private Vector3 targetPosition;
    private Vector3 direction;

    public override void Initialize(BulletSpawnRequest request)
    {
        base.Initialize(request);
        target = request.target;
        targetPosition = target?.model?.GetPosition() ?? request.position;
    }

    protected virtual void Update()
    {
        if (target != null && !target.IsDead())
        {
            Vector3 targetPos = target.model?.GetPosition() ?? targetPosition;
            targetPosition = targetPos;
            
            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            direction = Vector3.Lerp(direction, targetDirection, turnSpeed * Time.deltaTime);
            transform.position += direction * speed * Time.deltaTime;
            
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance <= hitDistance)
            {
                OnHit(target);
            }
        }
        else
        {
            Vector3 targetDirection = (targetPosition - transform.position).normalized;
            direction = Vector3.Lerp(direction, targetDirection, turnSpeed * Time.deltaTime);
            transform.position += direction * speed * Time.deltaTime;
            
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance <= hitDistance)
            {
                DestroyBullet();
            }
        }
    }
}
