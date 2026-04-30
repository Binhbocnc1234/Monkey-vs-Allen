using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitialize{
    public void Initialize();
}

public interface IPausable {
    public void SetEnable(bool toggle);
}

public interface IDestroyable {
    public void DestroyThis();
    public bool IsDead();
}

public interface IOnDestroy {
    public void OnDestroy();
}