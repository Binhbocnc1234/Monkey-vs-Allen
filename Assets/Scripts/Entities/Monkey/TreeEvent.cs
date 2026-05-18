using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TreeEvent : AnimatorEvent
{
    public Action OnShake;
    public void Shake(){
        OnShake?.Invoke();
    }
}
