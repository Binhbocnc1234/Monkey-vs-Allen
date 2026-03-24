using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MyCamera : Singleton<MyCamera> {
    //Camera's state
    [ReadOnly] public bool isMoving = false, isZoomUp = false;
    public Action OnFinishedMoving, OnFinishedZoomUp;
    public EventChannel eventChannel = new();
    protected float targetSize, targetZoomUp;
    protected Vector3 targetPos;
    [ReadOnly] public float initSize;
    [ReadOnly] public Vector3 initPos;
    public int moveSpeed = 5, zoomSpeed = 2;
    protected Camera cam;
    protected virtual void Start() {
        cam = Camera.main;
        initPos = transform.position;
        initSize = cam.orthographicSize;
    }

    protected void Update() {
        if(isMoving) {
            Vector2 diff = targetPos - this.transform.position;
            transform.Translate(diff.normalized * moveSpeed * Time.deltaTime);
            if(Vector2.Distance(transform.position, targetPos) < 0.1f) {
                isMoving = false;
                OnFinishedMoving?.Invoke();
            }
        }
        if(isZoomUp) {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
            if (Mathf.Abs(cam.orthographicSize - targetSize) < 0.1f) {
                isZoomUp = false;
                OnFinishedZoomUp?.Invoke();
            }
        }
    }
    public void SetTarget(Vector3 pos) {
        isMoving = true;
        targetPos = pos;
        initSize = cam.orthographicSize;
    }
    public void ZoomUp(float amount) {
        targetSize = initSize * amount;
        isZoomUp = true;
    }
    public void Reset(){
        cam.orthographicSize = initSize;
        transform.position = initPos;
    }

}
