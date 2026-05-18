using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Animator animator;
    public  
    void Awake() {
        Debug.Log("[Test] Awake");
        animator.CrossFade("Idle", 0.25f, 0, 0);
    }
    void Update() {
        if(animator.IsInTransition(0)) {
            Debug.Log("Animator in transition");
        }
    }
}
