using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MagnifyUI : MonoBehaviour {
    private Button button;
    public bool isZoomUp = false;
    public float zoomAmount = 0.5f;
    void Awake() {
        button = GetComponent<Button>();
    }
    public void OnClick() {
        if(isZoomUp) {
            MyCamera.Ins.ZoomUp(1);
        }
        else {
            MyCamera.Ins.ZoomUp(zoomAmount);
        }
        isZoomUp = !isZoomUp;
    }
    void Update() {
        button.interactable = !MyCamera.Ins.isZoomUp;
    }
}