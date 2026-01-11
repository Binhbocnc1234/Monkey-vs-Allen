using UnityEngine;

public class AlmanacCardUI : CardUI {
    // public 
    protected override void Start() {
        base.Start();
        OnClickEvent += (ui) => {
            SingletonRegister.Get<CardDescriptionUI>().Initialize(so);
        };
    }
    public override void ApplyCardSO(CardSO cardSO) {
        base.ApplyCardSO(cardSO);
        CardData cardData = PlayerData.GetCardDataById(cardSO.id);
        if(cardData.level == 0) {
            SetGreyOut();
        }

    }
    public void ShowCardDescription() {
        SingletonRegister.Get<CardDescriptionUI>().Initialize(so);
    }
}