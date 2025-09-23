using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridCamera : MyCamera
{
    public static new GridCamera Instance{ get; private set; }
    public bool canDraging = true;
    private IGrid grid;
    // Start is called before the first frame update
    protected new void Awake(){
        Instance = this;
    }
    protected new void Start()
    {
        base.Start();
        grid = IGrid.Instance; //Cannot be null because Start() is executed after Awake()
        // Calculate bounds of battlefield using GridSystem
        float battlefieldWidth = grid.width * grid.cellSize;
        float camZ = transform.position.z;
        camHalfWidth = cam.orthographicSize * cam.aspect;
        leftBound = -leftAndRightOffset;
        rightBound = battlefieldWidth + leftAndRightOffset;
    }
    protected new void Update(){
        base.Update();
        if(canDraging && !isMoving){ 
            HandleScrolling();
        }
    }
    public void MoveTowardPlayerHouse() {
        SetTarget(new Vector3(3, 5, -10));
    }
    public void MoveTowardEnemyHouse() {
        SetTarget(new Vector3(grid.GetCell(grid.width-1, grid.height-1).transform.position.x, 5, -10));
    }
    void HandleScrolling(){
        Vector3? dragDelta = null;

        // Mouse drag (PC)
        if(Input.mousePresent) {
            if(Input.GetMouseButtonDown(0)) {
                lastMousePosition = Input.mousePosition;
                isDragging = true;
            }
            else if(Input.GetMouseButtonUp(0)) {
                isDragging = false;
            }
            if(isDragging && Input.GetMouseButton(0)) {
                dragDelta = Input.mousePosition - lastMousePosition;
                lastMousePosition = Input.mousePosition;
            }
        }

        // Touch drag (Mobile)
        if(Input.touchSupported && Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began) {
                lastMousePosition = touch.position;
                isDragging = true;
            }
            else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                isDragging = false;
            }
            if(isDragging && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)) {
                dragDelta = (Vector3)touch.position - lastMousePosition;
                lastMousePosition = touch.position;
            }
        }

        // Apply drag if any
        if(dragDelta.HasValue) {
            float worldDeltaX = -dragDelta.Value.x * Camera.main.orthographicSize * 2f * Camera.main.aspect / Screen.width;
            Vector3 newPos = transform.position + new Vector3(worldDeltaX, 0, 0);
            newPos.x = Mathf.Clamp(newPos.x, leftBound, rightBound);
            transform.position = newPos;
        }
    }
}

