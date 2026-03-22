using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitialize{
    public void Initialize();
}

public interface IPausable {
    public void SetEnable(bool toggle);
}

/// <summary>
/// Dành cho những object không có MonoBehaviour, không có transform nhưng vẫn muốn cập nhật qua từng frame
/// </summary>
public interface IUpdatePerFrame{
    public void Update();
}
public interface IDestroyable {
    public void DestroyThis();
    public bool IsDead();
}

public interface IOnDestroy {
    public void OnDestroy();
}