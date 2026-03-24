using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUI : HideAndShowUI
{
    // Start is called before the first frame update
    public void InitializeFreePlay() {
        GetComponent<TMP_Text>().text = "Free Play Mode";
    }
    public void Initialize(Place place, int levelNumber)
    {
        GetComponent<TMP_Text>().text = $"{place} - {levelNumber + 1}";
    }
}
