using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : Singleton<MyCamera> {
    //Camera's state
    protected Vector3 lastMousePosition;
    protected bool isDragging = false;
    [ReadOnly] public bool isMoving = false;
    protected float targetSize;
    protected Vector3 targetPos;
    protected float initDistance, initSize;
    protected Vector3 initPos;
    //Technical stats
    public int lerpSpeed = 5;
    public float leftAndRightOffset;
    protected float leftBound;
    [ReadOnly] public float rightBound;
    protected float camHalfWidth;
    //References
    protected Camera cam;
    protected void Start() {
        cam = Camera.main;
        initPos = transform.position;
    }

    protected void Update() {
        if(isMoving) {
            Vector2 diff = (targetPos - this.transform.position);
            // if (diff.magnitude <= 5f)
            transform.Translate(diff.normalized*lerpSpeed*Time.deltaTime);
            cam.orthographicSize = (1-diff.magnitude / initDistance) * (targetSize - initSize) + initSize;
            if (Vector2.Distance(transform.position, targetPos) < 0.1f)
                isMoving = false;
        }
    }
    public void SetTarget(Vector3 pos, float zoomUp = 1) {
        isMoving = true;
        targetPos = pos;
        targetSize = Camera.main.orthographicSize/zoomUp;
        initDistance = (pos - transform.position).magnitude;
        initSize = cam.orthographicSize;
    }
    public void Reset(){
        cam.orthographicSize = initSize;
        transform.position = initPos;
    }

}
