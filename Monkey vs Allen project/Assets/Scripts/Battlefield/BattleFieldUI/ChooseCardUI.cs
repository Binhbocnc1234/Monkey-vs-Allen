using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseCardUI : Singleton<ChooseCardUI>
{
    public bool isChoosingCard = true;
    public TMP_Text tmp;
    public void OnClick() {
        isChoosingCard = !isChoosingCard;
        tmp.text = isChoosingCard ? "Let's rock" : "Choose Cards";
        var hsManager = UIManager.Ins.hideShowManager;
        if(isChoosingCard) {
            hsManager.Show("ownedCardContainer");
            hsManager.Show("letsrock");
            hsManager.Hide("level");
            BattleInfo.ChangeState(GameState.ChoosingCard);
        }
        else {
            hsManager.Hide("ownedCardContainer");
            hsManager.Hide("letsrock");
            hsManager.Show("level");
            CardManager.Ins.SetControlTeam(BattleInfo.chosenTeam);
            BattleInfo.ChangeState(GameState.Fighting);
        }
    }
}
