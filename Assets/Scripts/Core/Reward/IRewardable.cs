using UnityEngine;

[System.Serializable]
public abstract class Rewardable {
    [SerializeField] public int count = 1;
    public abstract void Reward(int count);
}
[System.Serializable]
public class CoinReward : Rewardable {
    public override void Reward(int count) {
        PlayerData.AddCoins(count);
    }
}
[System.Serializable]
public class CardReward : Rewardable {
    [SerializeField] public CardSO so;
    public override void Reward(int count) {
        PlayerData.GetCardDataById(so.id).Unlock();
    }
}
[System.Serializable]
public class CardShardReward : Rewardable {
    [SerializeField] public MonkeyCardSO so;
    public override void Reward(int count) {
        foreach(CardData data in PlayerData.MonkeyCards) {
            if (data.id == so.id) {
                data.shards += count;
                return;
            }
            
        }
    }
}

