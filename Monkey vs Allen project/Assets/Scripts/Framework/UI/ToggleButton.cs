using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class ToggleButton : MonoBehaviour {
    public Image targetImage;
    public Color normalColor = Color.white;
    public Color selectedColor;
    public float lerpSpeed = 10f;
    public UnityEvent<bool> OnSelected; 
    // public UnityEvent OnDeselected;
    private bool isSelected = false;
    private Button button;
    private Coroutine currentRoutine;

    void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        isSelected = !isSelected;

        if(currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(LerpColor(isSelected ? selectedColor : normalColor));
        OnSelected.Invoke(isSelected);
    }

    IEnumerator LerpColor(Color target) {
        while(Vector4.Distance(targetImage.color, target) > 0.01f) {
            targetImage.color = Color.Lerp(targetImage.color, target, Time.deltaTime * lerpSpeed);
            yield return null;
        }
        targetImage.color = target;
    }
    public bool IsSelected() => isSelected;
}