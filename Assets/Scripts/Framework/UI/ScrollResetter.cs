using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollResetter : MonoBehaviour {
    private ScrollRect scrollRect;
    void Awake() {
        scrollRect = GetComponent<ScrollRect>();
    }
    public void OpenScrollRect() {
        // Force layout rebuild BEFORE changing scroll value
        Canvas.ForceUpdateCanvases();

        scrollRect.verticalNormalizedPosition = 1f;

        // Optional safety rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            scrollRect.content
        );
    }
}