using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoadMono : Singleton<SaveAndLoadMono>
{
    protected override void Awake(){
        base.Awake();
        DontDestroyOnLoad(this);
        PlayerData.Load();
    }
    
}

