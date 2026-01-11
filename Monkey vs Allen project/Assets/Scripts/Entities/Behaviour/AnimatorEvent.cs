using System;
using UnityEngine;

public class AnimatorEvent : MonoBehaviour
{
    public event Action OnMakeDamage, EndDeathClip, OnFirstSkillFinished;
    public void MakeDamage(){
        OnMakeDamage?.Invoke();
    }
    public void SelfDestruction(){
        EndDeathClip?.Invoke();
    }
    public void FirstSkillFinished() {
        OnFirstSkillFinished.Invoke();
    }
}
