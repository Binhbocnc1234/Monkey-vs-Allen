
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescriptionHook : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("[DescriptionHook] Triggered");
        CardDescriptionUI.Ins.Initialize(NewCardUIManager.Ins.newCardSO);
    }
}
