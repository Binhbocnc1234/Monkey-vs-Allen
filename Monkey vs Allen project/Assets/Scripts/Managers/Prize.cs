using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class Prize : Singleton<Prize>{
    public bool isAnimating = false;
    public float animatingTime = 3f;
    private float t;
    void Start(){
        this.gameObject.SetActive(false);
    }
    public void OnMouseDown(){
        isAnimating = true;
        LeanTweenFake.Move(this.transform, Camera.main.transform.position, animatingTime);
    }
    public void Enable(Vector2 position){
        this.gameObject.SetActive(true);
        transform.position = position;
    }
    void Update(){
        if (isAnimating){
            t += Time.deltaTime;
            transform.localScale = Vector3.one * (1 + t/animatingTime);
            if (t >= animatingTime){
                isAnimating = false;
            }
        }
    }


}