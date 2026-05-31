using UnityEngine;
public abstract class BulletAppearance : MonoBehaviour
{
    protected Bullet b;
    protected Model model;
    public virtual void Initialize(Bullet bullet)
    {
        b = bullet;
        model = b.model;
    }
}