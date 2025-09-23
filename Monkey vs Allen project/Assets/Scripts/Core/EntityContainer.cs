using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityContainer : Singleton<EntityContainer>{

    public void ClearEntity(){
        foreach(Transform child in transform){
            if (!Application.isPlaying){
                DestroyImmediate(child.gameObject);
            }
            else{
                Destroy(child.gameObject);
            }
        }
    }
}