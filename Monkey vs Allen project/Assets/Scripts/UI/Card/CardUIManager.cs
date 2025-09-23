using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class CardUIManager : MonoBehaviour {
    [HideInInspector] public List<CardUI> cardUIs = new List<CardUI>();
    public CardUI prefab;
    public int count = 10;
    // public event
    protected virtual void Awake(){
        for(int i = 0; i < 10; ++i){
            cardUIs.Add(Instantiate(prefab, transform.position, Quaternion.identity, this.transform));
            cardUIs[i].OnClickEvent += OnCardClicked;
            cardUIs[i].index = i;
        }
    }
    public virtual void SetReferencedList(List<CardSO> cardSOs){
        prefab.gameObject.SetActive(false);
        int i = 0;
        foreach(CardSO cardSO in cardSOs){
            cardUIs[i].ApplyCardSO(cardSO);
            ++i;
        }
        for(; i < cardUIs.Count; ++i){
            cardUIs[i].RemoveCardSO();
        }
    }
    public CardUI FindCardUIBySO(CardSO so){
        if(so == null) { Debug.LogError("CardUIManager::FindCardUIBySO: so parameter is null"); }
        foreach(CardUI ui in cardUIs){
            if (ui.so == so){
                return ui;
            }
        }
        Debug.LogError($"Cannot find CardUI with so : {so.name}");
        return null;
    }
    protected abstract void OnCardClicked(CardUI cardUI);
}
