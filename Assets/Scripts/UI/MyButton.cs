using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private RectTransform upperLayer, lowerLayer;
    Vector2 originalUpperAnchoredPos;
    bool isHovering = false;

    void Start() {
        originalUpperAnchoredPos = upperLayer.anchoredPosition;
    }

    void LateUpdate() {
        if(isHovering) {
            // keep syncing in case lowerLayer moves/animates
            upperLayer.anchoredPosition = lowerLayer.anchoredPosition;
        }
        else{
            upperLayer.anchoredPosition = originalUpperAnchoredPos;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovering = false;
    }
}
