using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CardType{
    Tower,
    Enemy,
    Utility
}
public enum CoolDownType{
    None,
    Low,
    Medium,
    High
}

public enum CardRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
public enum CardAction{
    Summon
}
public enum CardCondition{ //Nếu vị trí đặt thẻ bài là hợp lệ thì sẽ hiển thị ảnh model mờ của card tại vị trí đó
    Summon,
    Buff,

}
[CreateAssetMenu(fileName ="NewCardSO", menuName = "ScriptableObject/Card")]
public class CardSO : MySO<CardSO>{
    public bool isPlayerCard = true;
    public string cardName = "Basic Tower";
    public int cost = 3;
    public float cooldown = 5;
    public Sprite sprite;
    public CardRarity cardRarity;
    [Header("Description")]
    [TextArea(3, 5)]
    public string description = "This is Basic Tower";
    public GameObject prefab;
    public CardCondition condition = CardCondition.Summon;
}
public abstract class ICard{
    public CardSO so;
    public int cost;
    public float cooldown;
    public Timer cooldownTimer;
    public abstract bool CanUseCard(Vector2Int gridPosition);
    public abstract void UseCard(Vector2Int gridPosition);
    public abstract float GetCooldownPercent();
    public abstract void ApplyCardSO(CardSO so);
    public abstract bool HaveEnoughBanana();

}