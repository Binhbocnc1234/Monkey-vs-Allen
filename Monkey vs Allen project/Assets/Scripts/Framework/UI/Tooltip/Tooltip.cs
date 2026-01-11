using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(RectTransform))]
public class Tooltip : MonoBehaviour {
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private float maxWidth = 200f;
    
    private RectTransform rectTransform;
    private Canvas canvas;
    
    void Awake() {
        SingletonRegister.Register(this);
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false);
    }
    
    public void Initialize(string text) {
        tmp.text = text;
        
        // Auto-size layout
        if (layoutElement != null) {
            layoutElement.preferredWidth = -1;
            layoutElement.preferredHeight = -1;
        }
        
        // Clamp width nếu text quá dài
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        if (rectTransform.sizeDelta.x > maxWidth && layoutElement != null) {
            layoutElement.preferredWidth = maxWidth;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
    
    public void Show(RectTransform triggerUI, Vector2 mousePos) {
        gameObject.SetActive(true);
        
        // Đặt position tooltip theo cursor
        rectTransform.position = mousePos + Vector2.up * 20f;
        
        // Kiểm tra nếu tooltip vượt quá screen bounds và điều chỉnh
        AdjustPositionToScreenBounds();
    }
    
    public void Hide() {
        gameObject.SetActive(false);
    }
    
    private void AdjustPositionToScreenBounds() {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        float rightEdge = corners[2].x;
        float leftEdge = corners[0].x;
        float topEdge = corners[1].y;
        float bottomEdge = corners[0].y;
        
        // Nếu vượt quá phải
        if (rightEdge > Screen.width) {
            rectTransform.position += Vector3.left * (rightEdge - Screen.width + 10f);
        }
        
        // Nếu vượt quá trái
        if (leftEdge < 0) {
            rectTransform.position += Vector3.right * (10f - leftEdge);
        }
        
        // Nếu vượt quá trên
        if (topEdge > Screen.height) {
            rectTransform.position += Vector3.down * (topEdge - Screen.height + 10f);
        }
    }
}