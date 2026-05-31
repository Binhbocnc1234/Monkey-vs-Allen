using UnityEngine;

public abstract class EntityAppearance : MonoBehaviour
{
    protected EntityModel model;
    protected IEntity e;
    public virtual void Initialize(EntityModel model)
    {
        this.model = model;
        e = model.e;
    }
}