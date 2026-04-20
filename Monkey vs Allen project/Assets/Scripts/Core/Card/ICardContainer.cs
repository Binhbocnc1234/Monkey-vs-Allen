using System.Collections.Generic;
using UnityEngine;
public interface ICardContainer {
    public List<IBattleCard> GetBattleCards(Team team);
    public static ICardContainer Ins{ get; protected set; }
}