using UnityEngine;

[System.Serializable]
public class EntityStat {
    [SerializeField] private ST statType;
    public float amount;
    public float multiplier = 1;
    public EntityStat(ST statType, float amount) {
        this.statType = statType;
        this.amount = amount;
    }
    public ST GetStatType() => statType;
}