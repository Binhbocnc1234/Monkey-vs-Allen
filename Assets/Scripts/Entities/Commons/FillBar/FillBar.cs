using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FillBar : MonoBehaviour {
    [SerializeField] private Image image;
    public void SetValue(float normalizedValue) {
        if(normalizedValue < 0 || normalizedValue > 1) {
            Debug.LogError("[FillBar] parameter 'normalizedValue' should range from 0 to 1");
        }
        image.fillAmount = normalizedValue;
    }
    public void ChangeColor(Color color) {
        image.color = color;
    }
}
