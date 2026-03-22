using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// This class will be added to BattleField right the time gameState == GameState.Fighting
/// When created, this class creates MonkeyCard corressponding to each cardUI
/// This class also handles Updating card's cooldown
/// </summary>
public class PlayerCardManager : MonoBehaviour
{
    public Transform container;
    public void InitializeForPlayer() {
        foreach(BattleCardUI cardUI in SingletonRegister.Get<ChosenCardManager>().cardUIs) {
            if(cardUI.so != null) {
                GameObject newObj = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().emptyGameObject, container);
                BattleCard battleCard = newObj.AddComponent<BattleCard>();
                battleCard.Initialize(cardUI.so, Team.Player, cardUI);
                BattleInfo.teamDict[Team.Player].cards.Add(battleCard);
            }
            else {
                cardUI.gameObject.SetActive(false);
            }
        }
    }
    public void InitializeForEnemy() {
        foreach(CardSO so in BattleInfo.levelSO.enemies) {
            GameObject newObj = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().emptyGameObject, container);
            BattleCard battleCard = newObj.AddComponent<BattleCard>();
            battleCard.Initialize(so, Team.Enemy);
            BattleInfo.teamDict[Team.Enemy].cards.Add(battleCard);
        }
    }
}
