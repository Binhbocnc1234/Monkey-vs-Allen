using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChangeFactionUI : MonoBehaviour {
    public TMP_Text factionTMP;
    public bool isMonkey = true;
    public void OnClick() {
        isMonkey = !isMonkey;
        factionTMP.text = isMonkey ? "Monkey" : "Alien";
        BattleInfo.chosenFaction = isMonkey ? Team.Player : Team.Enemy;
        SingletonRegister.Get<OwnedCardManager>().SetReferencedList(PlayerData.GetOwnedCard(isMonkey));
        SingletonRegister.Get<ChosenCardManager>().SetReferencedList(BattleInfo.GetChosenCardSOs());
    }
}