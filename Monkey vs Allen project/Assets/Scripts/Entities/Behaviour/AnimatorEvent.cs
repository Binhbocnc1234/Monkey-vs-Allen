using System;
using UnityEngine;

public class AnimatorEvent : MonoBehaviour
{
    public event Action OnMakeDamage, EndDeathClip;
    public void MakeDamage(){
        OnMakeDamage?.Invoke();
    }
    public void SelfDestruction(){
        EndDeathClip?.Invoke();
    }
    // public void
}
