using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// This class will be added to BattleField right the time gameState == GameState.Fighting
/// When created, this class creates MonkeyCard corressponding to each cardUI
/// This class also handles Updating card's cooldown
/// </summary>
public class PlayerCardManager : UpdateManager<BattleCard>
{
    public void Initialize() {
        foreach(BattleCardUI cardUI in SingletonRegister.Get<ChosenCardManager>().cardUIs){
            if (cardUI.so != null){
                BattleCard allyCard = new BattleCard(cardUI.so, Team.Player, cardUI);
                AddElement(allyCard);
                BattleInfo.chosenAllies.Add(allyCard);
            }
            else{
                cardUI.gameObject.SetActive(false);
            }
        }
    }

}
