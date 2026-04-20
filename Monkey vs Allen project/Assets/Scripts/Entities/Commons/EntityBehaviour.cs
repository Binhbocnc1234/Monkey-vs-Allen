using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityBehaviour : MonoBehaviour {
    private Entity _e;
    protected Entity e {
        get {
            if(_e == null) _e = GetComponent<Entity>();
            return _e;
        }
    }
}