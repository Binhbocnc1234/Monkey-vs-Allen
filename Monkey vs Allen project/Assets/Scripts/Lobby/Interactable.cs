using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour
{
    new BoxCollider2D collider;
    public event Action OnClick;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }
    public IEnumerator LoadCoroutine(){
        MyCamera.Ins.SetTarget(this.transform.position);
        MyCamera.Ins.ZoomUp(1.5f);
        yield return new WaitWhile(() => MyCamera.Ins.isMoving);
        yield return new WaitForSeconds(0.5f);
        OnClick?.Invoke();
    }
    protected virtual void OnMouseDown(){
        Debug.Log("Player click at a Interactable");
        StartCoroutine(LoadCoroutine());

    }
}
