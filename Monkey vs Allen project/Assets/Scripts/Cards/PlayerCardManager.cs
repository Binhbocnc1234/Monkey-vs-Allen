using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerCardManager : MonoBehaviour
{
    public Transform container;
    public Transform enemyContainer;
    public void Initialize() {
        container.DestroyAllChildren();
        enemyContainer.DestroyAllChildren();
        foreach(CardSO so in BattleInfo.teamDict[Team.Player].chosenCardSOs) {
            GameObject newObj = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().emptyGameObject, container);
            BattleCard battleCard = newObj.AddComponent<BattleCard>();
            battleCard.Initialize(so, Team.Player);
            BattleInfo.teamDict[Team.Player].cards.Add(battleCard);
        }
        foreach(CardSO so in BattleInfo.teamDict[Team.Enemy].chosenCardSOs) {
            GameObject newObj = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().emptyGameObject, enemyContainer);
            BattleCard battleCard = newObj.AddComponent<BattleCard>();
            battleCard.Initialize(so, Team.Enemy);
            BattleInfo.teamDict[Team.Enemy].cards.Add(battleCard);
        }
    }
}
