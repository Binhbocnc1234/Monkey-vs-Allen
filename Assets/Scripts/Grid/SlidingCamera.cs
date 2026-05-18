using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingCamera : Singleton<SlidingCamera> {
    protected Vector3 lastMousePosition, lastCameraPosition;
    public float offset;
    public float leftBound, rightBound;
    public bool enable {
        get { return enabled; }
        set{
            enabled = value;
            isDragging = false;
        }
    }
    [ReadOnly] public bool isDragging = false;
    private Camera cam;
    public void Initialize(float leftBound, float rightBound) {
        this.leftBound = leftBound - offset;
        this.rightBound = rightBound + offset;
        cam = Camera.main;
    }
    void Update() {
        if(MyCamera.Ins != null && MyCamera.Ins.isMoving || MyCamera.Ins.isZoomUp) {
            isDragging = false;
            return;
        }
        HandleScrolling();
    }
    void HandleScrolling(){
        float dragDelta = 0;

        // Mouse drag (PC)
        if(Input.mousePresent) {
            if(Input.GetMouseButtonDown(0)) {
                lastMousePosition = Input.mousePosition;
                lastCameraPosition = transform.position;
                isDragging = true;
            }
            else if(Input.GetMouseButtonUp(0)) {
                isDragging = false;
            }
            if(isDragging) {
                dragDelta = Input.mousePosition.x - lastMousePosition.x;
            }
        }

        // Touch drag (Mobile)
        // if(Input.touchSupported && Input.touchCount > 0) {
        //     Touch touch = Input.GetTouch(0);
        //     if(touch.phase == TouchPhase.Began) {
        //         lastMousePosition = touch.position;
        //         isDragging = true;
        //     }
        //     else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
        //         isDragging = false;
        //     }
        //     if(isDragging && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)) {
        //         dragDelta = (Vector3)touch.position - lastMousePosition;
        //     }
        // }
        if(isDragging) {
            float worldDelta = -dragDelta * cam.orthographicSize * 2f * cam.aspect / Screen.width;
            transform.position = lastCameraPosition + new Vector3(worldDelta, 0, 0);
        }
    }
}