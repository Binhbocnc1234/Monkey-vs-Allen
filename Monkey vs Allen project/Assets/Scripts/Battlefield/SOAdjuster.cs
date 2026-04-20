using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOAdjuster : MonoBehaviour {
    [ContextMenu("Adjust cost x10 times")]
    public void AdjustCost() {
        foreach(CardSO cardSO in Resources.LoadAll<CardSO>("")) {
            cardSO.cost /= 10;
        }
    }
}
