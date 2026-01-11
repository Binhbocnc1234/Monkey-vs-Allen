using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CardUIAppearance : MonoBehaviour{
    public Image frame, background, model;
    public TMP_Text cost;
    private RectTransform rectTrans;
    void Awake() {
        rectTrans = GetComponent<RectTransform>();
    }
    public void Initialize(CardSO so) {
        CardFrameSO cardFrameSO = CardFrameSO.GetObjectByRarity(so.cardRarity);
        frame.sprite = cardFrameSO.frame;
        background.sprite = cardFrameSO.background;
        model.sprite = so.sprite;
        cost.text = so.cost.ToString();
    }

}