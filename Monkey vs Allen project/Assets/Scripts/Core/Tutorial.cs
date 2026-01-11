using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Tutorial : MonoBehaviour{
    [TextArea(3, 5)]
    public string text;
    public Action OnTutorialComplete;
    public Action OnTutorialActivate;
    public virtual void Initialize(){
    }
    public virtual bool CanStartTutorial(){
        return true;
    }
    public virtual void StartTutorial(){
        OnTutorialActivate?.Invoke();
        enabled = true;
    }
    public virtual void CompleteTutorial(){
        OnTutorialComplete?.Invoke();
        enabled = false;
    }
}