using UnityEngine;

public class SpeedUpUI : MonoBehaviour {
    public bool isSpeedUp = false;
    public void OnClick() {
        isSpeedUp = !isSpeedUp;
        if(isSpeedUp) {
            Time.timeScale = 1.6f;
        }
        else {
            Time.timeScale = 1f;
        }
    }
}