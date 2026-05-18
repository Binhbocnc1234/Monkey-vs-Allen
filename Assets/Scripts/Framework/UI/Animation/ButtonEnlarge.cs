using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEnlarge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    RectTransform rt;

    void Awake() {
        rt = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData e) {
        LeanTween.cancel(gameObject);
        LeanTween.scale(rt, Vector3.one * 1.08f, 0.12f).setEaseOutQuad();
    }

    public void OnPointerExit(PointerEventData e) {
        LeanTween.cancel(gameObject);
        LeanTween.scale(rt, Vector3.one, 0.12f).setEaseOutQuad();
    }
}
