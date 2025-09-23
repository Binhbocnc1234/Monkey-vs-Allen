using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnedCardManager : CardUIManager
{
    public static OwnedCardManager Instance;
    protected override void Awake(){
        Instance = this;
        base.Awake();
    }
    protected override void OnCardClicked(CardUI ownedCardUI){
        CardUI chosenCard = ChosenCardManager.Instance.AddCard(ownedCardUI.so);
        // ownedCardUI.appearance.CreateCardMovement(ownedCardUI.GetComponent<RectTransform>().anchoredPosition, 
        // chosenCard.GetComponent<RectTransform>().anchoredPosition);
        ownedCardUI.SetGreyOut();
        Debug.Log("On Owned CardClick");
    }
}
