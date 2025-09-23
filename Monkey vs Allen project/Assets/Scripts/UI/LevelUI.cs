using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUI : HideAndShowUI
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TMP_Text>().text = $"{BattleInfo.place} - {BattleInfo.level}";
    }
}
