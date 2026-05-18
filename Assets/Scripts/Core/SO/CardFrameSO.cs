using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCardFrameSO", menuName = "ScriptableObject/CardFrame")]
public class CardFrameSO : MySO {
    public CardRarity rarity;
    public Sprite frame, background;
    public static CardFrameSO GetObjectByRarity(CardRarity rarity) {
        foreach(var so in SORegistry.Get<CardFrameSO>()) {
            if(so.rarity == rarity) { return so; }
        }
        Debug.LogError($"CardFrameSO::GetObjectByRarity: Cannot find SO of {rarity}");
        return null;
    }
}