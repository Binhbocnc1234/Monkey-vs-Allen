using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChangeFactionUI : MonoBehaviour
{
    public TMP_Text factionTMP;
    [ReadOnly] public bool isMonkey = true;
    public void OnClick() {
        isMonkey = !isMonkey;
        factionTMP.text = isMonkey ? "Monkey" : "Alien";
        SingletonRegister.Get<OwnedCardManager>().SetReferencedList(PlayerData.GetOwnedCard(isMonkey));
        SingletonRegister.Get<ChosenCardManager>().SetReferencedList(
            BattleInfo.teamDict[isMonkey ? Team.Player : Team.Enemy].cards.Select(e => e.GetSO()).ToList());
    }
}
