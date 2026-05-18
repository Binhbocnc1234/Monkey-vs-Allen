using UnityEngine;

public class StatUIPool : ObjectPool<StatUI> {
    // Thiết lập prefab, parent, initialSize qua Inspector trên GameObject chứa StatUIPool
    protected override void Awake() {
        base.Awake();
        SingletonRegister.Register(this);
    }
}