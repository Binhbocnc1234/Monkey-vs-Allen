using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CardUIAppearance : MonoBehaviour{
    public Image frame, background, model;
    public TMP_Text cost;
    private RectTransform rectTrans;
    void Awake(){
        rectTrans = GetComponent<RectTransform>();
    }
    public void CreateCardMovement(Vector2 anchoredStart, Vector2 anchoredEnd){
        // RectTransform newAppearance = Instantiate(this.rectTrans, Vector2.zero, Quaternion.identity, UIManager.Instance.transform);
        // newAppearance.localScale = Vector2.one;
        // newAppearance.anchoredPosition = anchoredStart;
        // StartCoroutine(LeanTweenFake.MoveUI(newAppearance, anchoredEnd, 0.5f));
        rectTrans.anchoredPosition = anchoredStart;
        StartCoroutine(LeanTweenFake.MoveUI(rectTrans, anchoredEnd, 0.5f));
        // rectTrans.
    }

}