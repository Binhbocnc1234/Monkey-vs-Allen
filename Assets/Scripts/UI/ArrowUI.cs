using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The arrow will oscillate in the direction you specify, you can set the magnitude variable, the higher the magnitude the longer the arrow trajectory and vice versa <br/>
/// In the Oxy coordinate system, if we consider the X axis as time and the Y axis as the position of the arrow in its trajectory, we will get an upside down parabola <br/>
/// That is, the Arrow will move quickly at a position far from the pointed object and move slowly when the arrow is close to the target
/// </summary>
public class ArrowUI : MonoBehaviour
{
    public RectTransform rect;
    public float magnitude = 50f;   // biên độ dao động
    public float speed = 2f;        // tần số dao động
    public Vector2 anchoredPosition;
    private Direction pointingDirection;
    private Vector2 basePos;
    private Vector2 pointingVec;

    void Awake() {
        rect = GetComponent<RectTransform>();
    }
    public static ArrowUI Instantiate(Direction direction, RectTransform pointingTarget, Transform parent){
        RectTransform newInstance = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().arrowUI, parent);
        ArrowUI arrowUI = newInstance.GetComponent<ArrowUI>();
        newInstance.gameObject.SetActive(true);
        
        arrowUI.pointingDirection = direction;
        switch (direction) {
            case Direction.Right: break;
            case Direction.Up:    newInstance.Rotate(0, 0, 90);  break;
            case Direction.Left:  newInstance.Rotate(0, 0, 180); break;
            case Direction.Down:  newInstance.Rotate(0, 0, 270); break;
            default: Debug.LogError("Bad pointingDirection"); break;
        }
        arrowUI.pointingVec = DirectionConverter.Convert(direction);
        arrowUI.basePos = arrowUI.pointingVec*(pointingTarget.anchoredPosition - new Vector2(20, 20));
        
        
        return arrowUI;
    }

    void Update() {
        //For testing purpose
        anchoredPosition = rect.anchoredPosition;

        float offset = -Mathf.Cos(Time.time * speed) * magnitude; 
        // cos -> parabol ngược trong khoảng nhỏ, 
        // tốc độ thay đổi nhanh xa target, chậm khi gần target
        rect.anchoredPosition = basePos + pointingVec * offset;
    }

    public RectTransform GetRectTrans(){
        return GetComponent<RectTransform>();
    }
    
}

