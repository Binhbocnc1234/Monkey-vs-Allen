using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


internal class AlmanacCardContainer : ObjectPool<AlmanacCardUI> {
    public void SetReferencedList(ReadOnlyCollection<CardData> cardDatas, bool isMonkeyCard) {
        Clear();
        for(int i = 0; i < cardDatas.Count; ++i) {
            if(cardDatas[i].discovered == false) { continue; }
            AlmanacCardUI obj = Get();
            if(cardDatas[i].level == 0) {
                obj.SetGreyOut(); 
            }
            if(isMonkeyCard) {
                obj.ApplyCardSO(SORegistry.Get<MonkeyCardSO>(cardDatas[i].id));
            }
            else {
                obj.ApplyCardSO(SORegistry.Get<EnemyCardSO>(cardDatas[i].id));
            }
        }
    }
}