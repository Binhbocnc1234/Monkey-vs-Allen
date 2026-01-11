using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class CardUIManager<T> : MonoBehaviour where T : CardUI {
    public Transform container;
    public List<T> cardUIs = new List<T>(); 
    public T prefab;
    public int count = 10;
    // public event
    protected virtual void Awake(){
        if(container == null) {
            container = this.transform;
        }
        foreach(Transform tr in container) {
            tr.gameObject.SetActive(false);
        }
        for(int i = 0; i < 10; ++i){
            cardUIs.Add(Instantiate(prefab, transform.position, Quaternion.identity, container));
            cardUIs[i].OnClickEvent += OnCardClicked;
            cardUIs[i].index = i;
        }
    }
    public virtual void SetReferencedList(List<CardSO> cardSOs){
        if(cardSOs.Count == 0) {
            Debug.Log("SetReferencedList: Warning: cardSO has no elements!");
        }
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
    public T FindCardUIBySO(CardSO so){
        if(so == null) { Debug.LogError("CardUIManager::FindCardUIBySO: so parameter is null"); }
        foreach(T ui in cardUIs){
            if (ui.so == so){
                return ui;
            }
        }
        Debug.LogError($"Cannot find CardUI with so : {so.name}");
        return null;
    }
    protected virtual void OnCardClicked(CardUI cardUI) { }
}

public class CardUIManager : CardUIManager<CardUI>{
    
}