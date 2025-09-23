using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="NewCardFrameSO", menuName = "ScriptableObject/CardFrame")]
public class CardFrameSO : MySO<CardFrameSO>{
    public CardRarity rarity;
    public Sprite frame, background;
    public static CardFrameSO GetObjectByRarity(CardRarity rarity){
        foreach(var so in container){
            if(so.rarity == rarity) { return so; }
        }
        return null;
    }
}