using UnityEngine;

public class TraitUIPool : ObjectPool<TraitUI> {
    // Thiết lập prefab, parent, initialSize qua Inspector trên GameObject chứa TraitUIPool
    protected override void Awake() {
        base.Awake();
        SingletonRegister.Register(this);
    }
}