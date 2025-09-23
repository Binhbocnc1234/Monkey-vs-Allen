using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Flourish : MonoBehaviour{
    public SpriteRenderer model;
    public void StartAnimation(){
        this.gameObject.SetActive(true);
    }
    public void EndAnimation(){
        // Instantiate()
    }
}