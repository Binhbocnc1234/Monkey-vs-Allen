using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashPanel : MonoBehaviour {
    public float speed = 1;
    Image panel;
    Color originalColor;
    void Start() {
        panel = GetComponent<Image>();
        originalColor = panel.color;
    }
    public IEnumerator FlashCorou() {
        Color white = originalColor;
        white.a = 0;
        while(true) {
            white.a += Time.deltaTime*speed;
            if(white.a >= 1) {
                break;
            }
            else {
                panel.color = white;
                yield return null;
            }

        }
    }
    public IEnumerator DeflashCorou(){
        Color white = originalColor;
        white.a = 1;
        while(true){
            white.a -= Time.deltaTime*speed;
            if (white.a <= 0){
                break;
            }
            else{
                panel.color = white;
                yield return null;
            }
            
        }  
    }
    public void StartFlash() {

    }
    public void DeFlash() {

    }
}
