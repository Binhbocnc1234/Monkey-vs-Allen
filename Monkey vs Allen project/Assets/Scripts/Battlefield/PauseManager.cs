using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PauseManager : Singleton<PauseManager>{
    public List<GameObject> pauseParent;
    private List<Behaviour> pauseList = new();
    /// <summary>
    /// Executed when players press Pause button, everything is paused, except: UI, 
    /// </summary>
    public void Pause(){
        GridCamera.Ins.canDraging = false;
        GetPauseList();
        foreach(var comp in pauseList){
            comp.enabled = false;
        }
    }
    public void DePause(){
        GridCamera.Ins.canDraging = true;
        foreach(var comp in pauseList){
            if (comp != null) comp.enabled = true;
        }
    }
    void GetPauseList(){
        foreach(GameObject parent in pauseParent){
            foreach (var comp in parent.GetComponents<Behaviour>())
            {
                if (comp.enabled == false || comp is IPausable || comp == this || comp is Image || comp is TMP_Text || comp is Button) 
                    continue;

                pauseList.Add(comp);
            }
            foreach (GameObject go in GetAllChildren(parent))
            {
                foreach (var comp in go.GetComponents<Behaviour>())
                {
                    if (comp.enabled == false || comp == this || comp is Image || comp is TMP_Text || comp is Button) 
                        continue;

                    pauseList.Add(comp);
                }
            }
        }
    }
    List<GameObject> GetAllChildren(GameObject parent)
    {
        var list = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            list.Add(child.gameObject);
            list.AddRange(GetAllChildren(child.gameObject)); // recursion
        }
        return list;
    }
}