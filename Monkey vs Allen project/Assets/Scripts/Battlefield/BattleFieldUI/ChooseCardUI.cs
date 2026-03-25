using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseCardUI : Singleton<ChooseCardUI>
{
    public bool isChoosingCard = true;
    public TMP_Text tmp;
    public event Action<bool> AfterClick;
    public void OnClick() {
        isChoosingCard = !isChoosingCard;
        tmp.text = isChoosingCard ? "Let's rock" : "Choose Cards";
        var hsManager = UIManager.Ins.hideShowManager;
        if(isChoosingCard) {
            hsManager.Show("ownedCardContainer");
            hsManager.Show("letsrock");
            hsManager.Show("changeFaction");
            hsManager.Hide("level");
            BattleInfo.ChangeState(GameState.ChoosingCard);
        }
        else {
            hsManager.Hide("ownedCardContainer");
            hsManager.Hide("letsrock");
            hsManager.Hide("changeFaction");
            hsManager.Show("level");
            BattleInfo.ChangeState(GameState.Fighting);
        }
        AfterClick?.Invoke(isChoosingCard);
    }
}
