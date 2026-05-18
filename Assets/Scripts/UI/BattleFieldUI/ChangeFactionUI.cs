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
        Team chosenTeam = isMonkey ? Team.Left : Team.Right;
        BattleInfo.chosenTeam = chosenTeam;
        SingletonRegister.Get<ChosenCardManager>().SetReferencedList(BattleInfo.teamDict[chosenTeam].chosenCardSOs);
        SingletonRegister.Get<OwnedCardManager>().SetReferencedList(PlayerData.GetOwnedCard(isMonkey));
        if (BattleInfo.gameState == GameState.Fighting) {
            SingletonRegister.Get<ChosenCardManager>().SetControlTeam(BattleInfo.chosenTeam);
        }
    }
}