using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowContainer : ObjectPool<Shadow>
{
    protected override void Awake() {
        base.Awake();
        SingletonRegister.Register(this);
    }
}
