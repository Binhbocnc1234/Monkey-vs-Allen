using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;


public class TutorialManager : Singleton<TutorialManager>{
    public RectTransform tutorialLineWrapper;
    public TMP_Text tutorialLine;
    public Transform tutorialContainer;
    List<Tutorial> tutorials = new List<Tutorial>();
    [ReadOnly] public int activeTutorialIndex = -1;
    public void Initialize(){
        if(enabled == false) { return; }
        TechnicalInfo.isTutorial = true;
        foreach(Tutorial tutor in BattleInfo.levelSO.tutorials){
            var newTutor = Instantiate(tutor, tutorialContainer);
            tutorials.Add(newTutor);
            newTutor.enabled = false;
            newTutor.OnTutorialComplete += CompleteTutorial;
            newTutor.OnTutorialActivate += () => { 
                ShowText(GetActiveTutorial().text);
                GridCamera.Ins.canDraging = false;
            };
        }
        tutorialLineWrapper.gameObject.SetActive(false);
        NextTutorial();
    }
    void Update(){
   
    }
    void CompleteTutorial(){
        HideText();
        GridCamera.Ins.canDraging = true;
        NextTutorial();
    }
    void NextTutorial(){
        activeTutorialIndex++;
        
        if (activeTutorialIndex == tutorials.Count){
            TechnicalInfo.isTutorial = false;
            Debug.Log("Finish tutorial");
            return;
        }
        Debug.Log($"Next tutorial {activeTutorialIndex}, {GetActiveTutorial().gameObject.name}");
        ShowText(GetActiveTutorial().text);
        GetActiveTutorial().Initialize();
    }
    void ShowText(string text){
        tutorialLine.text = text;
        tutorialLineWrapper.gameObject.SetActive(true);
    }
    void HideText(){
        tutorialLineWrapper.gameObject.SetActive(false);
    }
    Tutorial GetActiveTutorial(){
        return tutorials[activeTutorialIndex];
    }

}