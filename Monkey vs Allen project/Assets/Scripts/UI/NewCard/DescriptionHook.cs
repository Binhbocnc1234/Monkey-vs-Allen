
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescriptionHook : MonoBehaviour
{
    public void OnClick() {
        CardDescriptionUI.Ins.Initialize(NewCardUIManager.Ins.newCardSO);
    }
}
