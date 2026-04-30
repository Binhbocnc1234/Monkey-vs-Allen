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
        BattleInfo.OnChosenTeamChanged += () => {
            if (BattleInfo.gameState == GameState.Fighting) {
                SetControlTeam(BattleInfo.chosenTeam);
            }
        };
    }
    public void Initialize() {
        container.DestroyAllChildren();
        enemyContainer.DestroyAllChildren();
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
    public void SetControlTeam(Team team) {
        foreach(Transform tr in GetContainer(team)) {
            BattleCard card = tr.GetComponent<BattleCard>();
            SingletonRegister.Get<ChosenCardManager>().FindCardUIBySO(card.GetSO()).card = card;
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
