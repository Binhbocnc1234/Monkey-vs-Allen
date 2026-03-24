using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseCardUI : MonoBehaviour
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
            hsManager.Show("changeFaction");
            hsManager.Hide("level");
        }
        else {
            hsManager.Hide("ownedCardContainer");
            hsManager.Hide("letsrock");
            hsManager.Hide("changeFaction");
            hsManager.Show("level");
            foreach(CardSO chosenCard in BattleInfo.choosenCardSOs) {
                
            }
        }
    }
}
