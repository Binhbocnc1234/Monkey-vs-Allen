using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public abstract class CardSO : MySO{
    public static readonly int[] UPGRADE_THRESHOLD = new int[3] { 10, 30, 100 };
    public static int maxLevel;
    [FormerlySerializedAs("cardName")]
    public string cardName;
    public EntitySO entitySO;
    [FormerlySerializedAs("cost")]
    public int cost = 3;
    [FormerlySerializedAs("cooldownType")]
    public CoolDownType cooldownType;
    [FormerlySerializedAs("sprite")]
    public Sprite sprite;
    [FormerlySerializedAs("cardRarity")]
    public CardRarity cardRarity;
    [Header("Upgrades")]
    public LocalizedDescription description;

}
