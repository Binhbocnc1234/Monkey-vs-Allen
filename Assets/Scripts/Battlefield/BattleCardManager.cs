using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BattleCardManager : Singleton<BattleCardManager>, ICardContainer
{
    public Transform container;
    public Transform enemyContainer;
    protected override void Awake() {
        base.Awake();
        ICardContainer.Ins = this;
    }
    public void CreateBattleCard() {
        container.DestroyAllChildren();
        enemyContainer.DestroyAllChildren();
        BattleInfo.teamDict[Team.Left].cards.Clear();
        BattleInfo.teamDict[Team.Right].cards.Clear();
        foreach(CardSO so in BattleInfo.teamDict[Team.Left].chosenCardSOs) {
            GameObject newObj = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().emptyGameObject, container);
            newObj.name = $"Battle Card - {so.name}";
            BattleCard battleCard = newObj.AddComponent<BattleCard>();
            battleCard.Initialize(so, Team.Left, PlayerData.GetCardDataById(so.id).level);
            BattleInfo.teamDict[Team.Left].cards.Add(battleCard);
        }
        foreach(CardSO so in BattleInfo.teamDict[Team.Right].chosenCardSOs) {
            GameObject newObj = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().emptyGameObject, enemyContainer);
            newObj.name = $"Battle Card - {so.name}";
            BattleCard battleCard = newObj.AddComponent<BattleCard>();
            battleCard.Initialize(so, Team.Right, PlayerData.GetCardDataById(so.id).level);
            BattleInfo.teamDict[Team.Right].cards.Add(battleCard);
        }
    }
    Transform GetContainer(Team team) {
        return team == Team.Left ? container : enemyContainer;
    }
    public List<IBattleCard> GetBattleCards(Team team) {
        List<IBattleCard> battleCards = new();
        foreach(Transform tr in GetContainer(team)) {
            battleCards.Add(tr.GetComponent<IBattleCard>());
        }
        return battleCards;
    }
}
