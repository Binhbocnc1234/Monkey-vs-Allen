using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardAppearance : MonoBehaviour
{
    public SpriteRenderer frame, background, model;
    public TMP_Text cost;
    private RectTransform rectTrans;
    void Awake() {
        rectTrans = GetComponent<RectTransform>();
    }
    public void ApplyCardSO(CardSO so) {
        CardFrameSO cardFrameSO = CardFrameSO.GetObjectByRarity(so.cardRarity);
        frame.sprite = cardFrameSO.frame;
        // background.

    }
}
