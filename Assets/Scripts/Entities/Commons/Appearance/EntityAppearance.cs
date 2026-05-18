using UnityEngine;

public class EntityAppearance : MonoBehaviour
{
    protected Model model;
    protected IEntity e;
    public virtual void Initialize()
    {
        model = GetComponent<Model>();
        e = model.e;
    }
}