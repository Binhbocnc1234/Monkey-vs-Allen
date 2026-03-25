using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnedCardManager : CardUIManager<CardUI>
{
    protected override void Awake() {
        base.Awake();
        SingletonRegister.Register(this);
    }
    public override void SetReferencedList(List<CardSO> lst) {
        base.SetReferencedList(lst);
        foreach(CardUI ui in cardUIs) {
            ui.RemoveGreyOut();
        }
        foreach(CardSO so in BattleInfo.GetChosenCardSOs()) {
            FindCardUIBySO(so).SetGreyOut();
        }
    }
    protected override void OnCardClicked(CardUI ownedCardUI){
        SingletonRegister.Get<ChosenCardManager>().AddCard(ownedCardUI.so);
        BattleInfo.GetChosenCardSOs().Add(ownedCardUI.so);
        // ownedCardUI.appearance.CreateCardMovement(ownedCardUI.GetComponent<RectTransform>().anchoredPosition, 
        // chosenCard.GetComponent<RectTransform>().anchoredPosition);
        ownedCardUI.SetGreyOut();
        Debug.Log("On Owned CardClick");
    }
}
