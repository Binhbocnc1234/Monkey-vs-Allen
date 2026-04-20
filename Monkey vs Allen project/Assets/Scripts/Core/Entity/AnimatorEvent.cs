using System;
using UnityEngine;

public class AnimatorEvent : MonoBehaviour
{
    public Action OnMakeDamage, EndDeathClip;
    public Action<AnimatorStateInfo> OnAnimationFinished;
    private Animator animator;
    [Range(0f, 1f), SerializeField] private float completionPercent;
    void Awake() {
        animator = GetComponent<Animator>();
        OnMakeDamage = null;
        EndDeathClip = null;
        OnAnimationFinished = null;
    }
    // void Update() {
    //     AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
    //     if(state.IsName(GetComponent<Model>().currentStateName) && state.normalizedTime >= completionPercent && !animator.IsInTransition(0)) {
    //         OnAnimationFinished?.Invoke();
    //     }
    // }
    public void MakeDamage(){
        OnMakeDamage?.Invoke();
    }
    public void SelfDestruction(){
        EndDeathClip?.Invoke();
    }
}
