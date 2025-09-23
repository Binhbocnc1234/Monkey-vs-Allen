using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType{
    Lobby,
    Collections,
    Battlefield,
    Other,
}

public class CustomSceneManager : Singleton<MonoBehaviour>
{

    public void ChangeScene(SceneType sceneType){
        switch(sceneType){
            case SceneType.Lobby:
                ChangeScene("Lobby"); break;
            case SceneType.Collections:
                ChangeScene("Collections"); break;
            case SceneType.Battlefield:
                ChangeScene("Battlefield"); break;
            default:
                Debug.LogError("CustomSceneManager::ChangeScene: Invalid parameter sceneType"); break;
        }
        
    }
    public void ChangeScene(string name){
        SceneManager.LoadScene(name);
    }
}
