using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChangeFactionUI : Singleton<ChangeFactionUI> {
    public TMP_Text factionTMP;
    public bool isMonkey = true;
    public void OnClick() {
        isMonkey = !isMonkey;
        factionTMP.text = isMonkey ? "Monkey" : "Alien";
        BattleInfo.chosenTeam = isMonkey ? Team.Player : Team.Enemy;
        SingletonRegister.Get<OwnedCardManager>().SetReferencedList(PlayerData.GetOwnedCard(isMonkey));
        SingletonRegister.Get<ChosenCardManager>().SetReferencedList(BattleInfo.GetChosenCardSOs());
        if(BattleInfo.gameState == GameState.Fighting) {
            CardManager.Ins.SetControlTeam(isMonkey ? Team.Player : Team.Enemy);
        }
    }
}