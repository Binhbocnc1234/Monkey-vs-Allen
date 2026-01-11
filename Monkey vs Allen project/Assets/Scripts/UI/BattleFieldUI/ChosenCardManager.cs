using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ChosenCardManager shouldn't control MonkeyCard, because it was designed purely for UI, there should have other manager controll it
/// </summary>
public class ChosenCardManager : CardUIManager<BattleCardUI>
{
    [ReadOnly] public int lastActiveCardIndex = -1;
    protected override void Awake(){
        SingletonRegister.Register(this);
        base.Awake();
    }
    public CardUI GetFirstEmptyCardUI(){
        return cardUIs[lastActiveCardIndex+1];
    }
    public CardUI AddCard(CardSO so){
        lastActiveCardIndex++;
        cardUIs[lastActiveCardIndex].ApplyCardSO(so);
        return cardUIs[lastActiveCardIndex];
    }
    public override void SetReferencedList(List<CardSO> cardSOs) {
        base.SetReferencedList(cardSOs);
        lastActiveCardIndex = cardSOs.Count - 1;
        foreach(CardSO so in cardSOs) {
            SingletonRegister.Get<OwnedCardManager>().FindCardUIBySO(so).SetGreyOut();
        }
    }
    protected override void OnCardClicked(CardUI _chosenCardUI) {
        BattleCardUI chosenCardUI = _chosenCardUI.GetComponent<BattleCardUI>();
        if (BattleInfo.gameState == GameState.ChoosingCard){
            if (BattleInfo.levelSO.choosenCardsBySystem.Contains(_chosenCardUI.so)){ return; }
            BattleInfo.choosenCardSOs.Remove(_chosenCardUI.so);
            CardUI correspondingOwnedCardUI = SingletonRegister.Get<OwnedCardManager>().FindCardUIBySO(chosenCardUI.so);
            correspondingOwnedCardUI.RemoveGreyOut(); 
            chosenCardUI.RemoveCardSO(); 
            //If removed card is from middle, push the CardUIs behind forward 1 step to fill the gap
            for(int i = chosenCardUI.index + 1; i <= lastActiveCardIndex; ++i){
                cardUIs[i - 1].ApplyCardSO(cardUIs[i].so);
            }
            cardUIs[lastActiveCardIndex].RemoveCardSO();
            lastActiveCardIndex--;
        }
    }
}
