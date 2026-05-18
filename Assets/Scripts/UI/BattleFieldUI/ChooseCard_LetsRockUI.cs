using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseCard_LetsRockUI : Singleton<ChooseCard_LetsRockUI>
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
            BattleInfo.ChangeState(GameState.Fighting);
        }
    }
}
