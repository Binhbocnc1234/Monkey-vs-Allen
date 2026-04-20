using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CardManager : Singleton<CardManager>, ICardContainer
{
    public Transform container;
    public Transform enemyContainer;
    protected override void Awake() {
        base.Awake();
        ICardContainer.Ins = this;
    }
    public void Initialize() {
        container.DestroyAllChildren();
        enemyContainer.DestroyAllChildren();
        foreach(CardSO so in BattleInfo.teamDict[Team.Player].chosenCardSOs) {
            GameObject newObj = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().emptyGameObject, container);
            newObj.name = $"Battle Card - {so.name}";
            BattleCard battleCard = newObj.AddComponent<BattleCard>();
            battleCard.Initialize(so, Team.Player);
            BattleInfo.teamDict[Team.Player].cards.Add(battleCard);
        }
        foreach(CardSO so in BattleInfo.teamDict[Team.Enemy].chosenCardSOs) {
            GameObject newObj = Instantiate(SingletonRegister.Get<PrefabRegisterSO>().emptyGameObject, enemyContainer);
            newObj.name = $"Battle Card - {so.name}";
            BattleCard battleCard = newObj.AddComponent<BattleCard>();
            battleCard.Initialize(so, Team.Enemy);
            BattleInfo.teamDict[Team.Enemy].cards.Add(battleCard);
        }
    }
    public void SetControlTeam(Team team) {
        foreach(Transform tr in GetContainer(team)) {
            BattleCard card = tr.GetComponent<BattleCard>();
            card.SetCardUI(SingletonRegister.Get<ChosenCardManager>().FindCardUIBySO(card.GetSO()));
        }
    }
    Transform GetContainer(Team team) {
        return team == Team.Player ? container : enemyContainer;
    }
    public List<IBattleCard> GetBattleCards(Team team) {
        List<IBattleCard> battleCards = new();
        foreach(Transform tr in GetContainer(team)) {
            battleCards.Add(tr.GetComponent<IBattleCard>());
        }
        return battleCards;
    }
}
