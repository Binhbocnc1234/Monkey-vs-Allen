using UnityEngine;

[RequireComponent(typeof(IEntity))]
public class EntityBehaviour : MonoBehaviour {
    private IEntity _e;
    protected IEntity e {
        get {
            if(_e == null) _e = GetComponent<IEntity>();
            return _e;
        }
    }
}