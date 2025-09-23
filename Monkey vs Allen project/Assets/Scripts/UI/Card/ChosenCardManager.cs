using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChosenCardManager : CardUIManager
{
    [ReadOnly] public int lastActiveCardIndex = -1;
    public static ChosenCardManager Instance;
    protected override void Awake(){
        Instance = this;
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
        foreach(CardSO so in cardSOs){
            OwnedCardManager.Instance.FindCardUIBySO(so).SetGreyOut();
        }
    }
    protected override void OnCardClicked(CardUI chosenCardUI) {
        if (BattleInfo.state == GameState.ChoosingCard){
            CardUI correspondingOwnedCardUI = OwnedCardManager.Instance.FindCardUIBySO(chosenCardUI.so);
            correspondingOwnedCardUI.RemoveGreyOut(); 
            chosenCardUI.RemoveCardSO(); 
            //If removed card is from middle, push the CardUIs behind forward 1 step to fill the gap
            for(int i = chosenCardUI.index + 1; i <= lastActiveCardIndex; ++i){
                cardUIs[i - 1].ApplyCardSO(cardUIs[i].so);
            }
            cardUIs[lastActiveCardIndex].RemoveCardSO();
            lastActiveCardIndex--;
        }
        else if (BattleInfo.state == GameState.Fighting){
            if (chosenCardUI.card.cooldownTimer.isEnd && chosenCardUI.card.HaveEnoughBanana()){
                PointerUI.Instance.UpdateHoldingCard(chosenCardUI.card);
            }
            
            
        }
    }
}
